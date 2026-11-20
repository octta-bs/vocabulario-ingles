using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>Una respuesta del diagnóstico tal como llega del formulario.</summary>
public record RespuestaDiagnostico(int PalabraId, TipoEjercicio Tipo, string? Respuesta);

/// <summary>
/// Diagnóstico de nivel: un examen corto de vocabulario, gramática y escuchar, de fácil a difícil, que estima tu
/// nivel en cada área, te dice tus puntos débiles y qué contenido le falta a la app para tu nivel.
/// No toca tu repaso: solo guarda el resultado para compararlo después.
/// </summary>
public class DiagnosticoService
{
    // Cuántas preguntas de vocabulario por nivel (las palabras sueltas llegan hasta A2; B1 y B2 son frases).
    private static readonly (NivelMcer Nivel, int Cantidad)[] Vocabulario =
    {
        (NivelMcer.PreA1, 2), (NivelMcer.A1, 2), (NivelMcer.A2, 2), (NivelMcer.B1, 1), (NivelMcer.B2, 1)
    };

    // Un dictado por nivel.
    private static readonly NivelMcer[] Escucha = { NivelMcer.A1, NivelMcer.A2, NivelMcer.B1, NivelMcer.B2 };

    // Para dar un nivel por bueno hay que acertar al menos la mitad de sus preguntas.
    public const double Minimo = 0.5;

    private static readonly JsonSerializerOptions Json = new();

    private readonly AppDbContext _db;
    private readonly GeneradorEjercicios _generador;
    private readonly TimeProvider _reloj;

    public DiagnosticoService(AppDbContext db, GeneradorEjercicios generador, TimeProvider reloj)
    {
        _db = db;
        _generador = generador;
        _reloj = reloj;
    }

    /// <summary>Arma un examen nuevo: vocabulario, gramática (un tema por pregunta) y escuchar, cada uno de fácil a difícil.</summary>
    public async Task<List<PreguntaDiagnostico>> ArmarAsync(Random azar)
    {
        var palabras = await _db.Palabras
            .Include(p => p.Categoria)
            .Where(p => p.Categoria!.Nombre != LecturasService.NombreCategoria)
            .ToListAsync();
        var usadas = new HashSet<int>();
        var preguntas = new List<PreguntaDiagnostico>();

        Palabra? Elegir(Func<Palabra, bool> filtro)
        {
            var candidatas = palabras.Where(p => !usadas.Contains(p.Id) && filtro(p)).ToList();
            if (candidatas.Count == 0)
            {
                return null;
            }

            var elegida = candidatas[azar.Next(candidatas.Count)];
            usadas.Add(elegida.Id);
            return elegida;
        }

        void Agregar(AreaDiagnostico area, Palabra? palabra, TipoEjercicio tipo)
        {
            if (palabra == null)
            {
                return;
            }

            var ejercicio = _generador.Crear(palabra, tipo);
            // Solo si de verdad se pudo armar ese tipo (por ejemplo, completar necesita la palabra clave en la frase).
            if (ejercicio.Tipo == tipo)
            {
                preguntas.Add(new PreguntaDiagnostico(area, palabra.Categoria!.Nivel, ejercicio));
            }
        }

        // Vocabulario: palabras sueltas (escribirlas) hasta A2, y frases de trabajo (completar) en B1 y B2.
        foreach (var (nivel, cantidad) in Vocabulario)
        {
            for (var i = 0; i < cantidad; i++)
            {
                if (nivel <= NivelMcer.A2)
                {
                    Agregar(AreaDiagnostico.Vocabulario,
                        Elegir(p => p.Categoria!.Nivel == nivel && p.Tipo is TipoPalabra.Sustantivo or TipoPalabra.Verbo),
                        TipoEjercicio.EscribirPalabra);
                }
                else
                {
                    Agregar(AreaDiagnostico.Vocabulario,
                        Elegir(p => p.Categoria!.Nivel == nivel && p.EsFrase),
                        TipoEjercicio.CompletarFrase);
                }
            }
        }

        // Gramática: una frase de cada tema, en el orden de estudio, para completar su parte gramatical.
        foreach (var tema in TemasGramatica.Todos)
        {
            Agregar(AreaDiagnostico.Gramatica,
                Elegir(p => p.EsGramatica && p.Categoria!.Nombre == tema.Categoria),
                TipoEjercicio.CompletarFrase);
        }

        // Escuchar: el ejemplo de una palabra en A1 y A2; una frase de trabajo en B1 y B2.
        foreach (var nivel in Escucha)
        {
            if (nivel <= NivelMcer.A2)
            {
                Agregar(AreaDiagnostico.Escucha,
                    Elegir(p => p.Categoria!.Nivel == nivel && p.Tipo is TipoPalabra.Sustantivo or TipoPalabra.Verbo
                                && !string.IsNullOrWhiteSpace(p.Ejemplo)),
                    TipoEjercicio.DictadoFrase);
            }
            else
            {
                Agregar(AreaDiagnostico.Escucha,
                    Elegir(p => p.Categoria!.Nivel == nivel && p.EsFrase),
                    TipoEjercicio.DictadoPalabra);
            }
        }

        return preguntas;
    }

    /// <summary>Califica las respuestas, guarda el diagnóstico y regresa su id.</summary>
    public async Task<int?> CalificarYGuardarAsync(IReadOnlyList<RespuestaDiagnostico> respuestas)
    {
        var ids = respuestas.Select(r => r.PalabraId).ToHashSet();
        var palabras = await _db.Palabras.Include(p => p.Categoria).Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        var calificadas = respuestas
            .Where(r => palabras.ContainsKey(r.PalabraId))
            .Select(r =>
            {
                var palabra = palabras[r.PalabraId];
                var ejercicio = _generador.Crear(palabra, r.Tipo);
                return (Pregunta: new PreguntaDiagnostico(AreaDe(ejercicio), palabra.Categoria!.Nivel, ejercicio),
                    Acerto: ComparadorRespuestas.SonIguales(r.Respuesta, ejercicio.Respuesta));
            })
            .ToList();

        if (calificadas.Count == 0)
        {
            return null;
        }

        var resultado = Calificar(calificadas);
        var diagnostico = new Diagnostico
        {
            Fecha = _reloj.GetUtcNow().UtcDateTime,
            Aciertos = resultado.Aciertos,
            Total = resultado.Total,
            NivelVocabulario = resultado.Area(AreaDiagnostico.Vocabulario).Nivel,
            NivelGramatica = resultado.Area(AreaDiagnostico.Gramatica).Nivel,
            NivelEscucha = resultado.Area(AreaDiagnostico.Escucha).Nivel,
            Detalle = JsonSerializer.Serialize(resultado, Json)
        };
        _db.Diagnosticos.Add(diagnostico);
        await _db.SaveChangesAsync();
        return diagnostico.Id;
    }

    /// <summary>De qué área es un ejercicio: los dictados son de escuchar; las frases de gramática, de gramática.</summary>
    public static AreaDiagnostico AreaDe(Ejercicio ejercicio) =>
        ejercicio.EsDictado ? AreaDiagnostico.Escucha
        : ejercicio.Palabra.EsGramatica ? AreaDiagnostico.Gramatica
        : AreaDiagnostico.Vocabulario;

    /// <summary>
    /// Calcula el resultado. En cada área, tu nivel es el más alto al que llegas acertando al menos la mitad de cada
    /// nivel, sin saltarte ninguno (si fallas A1, no cuenta que aciertes B1 por suerte).
    /// </summary>
    public static ResultadoDiagnostico Calificar(IReadOnlyList<(PreguntaDiagnostico Pregunta, bool Acerto)> calificadas)
    {
        var areas = Enum.GetValues<AreaDiagnostico>()
            .Select(area =>
            {
                var delArea = calificadas.Where(c => c.Pregunta.Area == area).ToList();
                var porNivel = delArea.GroupBy(c => c.Pregunta.Nivel).OrderBy(g => g.Key).ToList();
                NivelMcer? nivel = null;
                foreach (var grupo in porNivel)
                {
                    if (grupo.Count(c => c.Acerto) < Minimo * grupo.Count())
                    {
                        break;
                    }

                    nivel = grupo.Key;
                }

                var maximo = porNivel.Count == 0 ? NivelMcer.PreA1 : porNivel[^1].Key;
                return new ResultadoArea(area, nivel, delArea.Count(c => c.Acerto), delArea.Count, maximo);
            })
            .ToList();

        // Puntos débiles: lo que fallaste, del nivel más básico al más alto. En gramática, cada tema por separado.
        var falladas = calificadas.Where(c => !c.Acerto).ToList();
        var debiles = new List<(NivelMcer Nivel, int Orden, SugerenciaDiagnostico Sugerencia)>();
        var temasFallados = new List<string>();
        foreach (var tema in TemasGramatica.Todos)
        {
            if (falladas.Any(c => c.Pregunta.Area == AreaDiagnostico.Gramatica
                                  && c.Pregunta.Ejercicio.Palabra.Categoria?.Nombre == tema.Categoria))
            {
                temasFallados.Add(tema.Clave);
                debiles.Add((tema.Nivel, 0, new SugerenciaDiagnostico(
                    $"Gramática: repasa el tema \"{tema.Titulo}\".", "Gramatica", "Tema", tema.Clave)));
            }
        }

        var vocabulario = falladas.Where(c => c.Pregunta.Area == AreaDiagnostico.Vocabulario).ToList();
        if (vocabulario.Count > 0)
        {
            var nivel = vocabulario.Min(c => c.Pregunta.Nivel);
            debiles.Add((nivel, 1, new SugerenciaDiagnostico(
                $"Vocabulario de nivel {nivel.Texto()}: aprende palabras nuevas en tu repaso.", "Repaso", "Nuevas")));
        }

        var escucha = falladas.Where(c => c.Pregunta.Area == AreaDiagnostico.Escucha).ToList();
        if (escucha.Count > 0)
        {
            var nivel = escucha.Min(c => c.Pregunta.Nivel);
            debiles.Add((nivel, 2, new SugerenciaDiagnostico(
                $"Escuchar (nivel {nivel.Texto()}): practica shadowing para entrenar el oído.", "Practica", "Shadowing")));
        }

        var puntosDebiles = debiles.OrderBy(d => d.Nivel).ThenBy(d => d.Orden).Select(d => d.Sugerencia).Take(3).ToList();

        // Lo que le falta a la app: si ya llegas al nivel más alto que tiene un área, hace falta contenido más difícil.
        var leFalta = areas
            .Where(a => a.Total > 0 && a.Nivel == a.NivelMaximo && a.NivelMaximo < NivelMcer.C2)
            .Select(a => $"{a.Area.Texto()}: ya dominas lo más difícil que tiene la app ({a.NivelMaximo.Texto()}). " +
                         $"Hace falta contenido de nivel {(a.NivelMaximo + 1).Texto()}.")
            .ToList();

        return new ResultadoDiagnostico(areas, puntosDebiles, leFalta, temasFallados);
    }

    public async Task<List<Diagnostico>> HistorialAsync()
    {
        return await _db.Diagnosticos.OrderByDescending(d => d.Fecha).ToListAsync();
    }

    public async Task<(Diagnostico Diagnostico, ResultadoDiagnostico Resultado, Diagnostico? Anterior)?> ObtenerAsync(int id)
    {
        var diagnostico = await _db.Diagnosticos.FirstOrDefaultAsync(d => d.Id == id);
        if (diagnostico == null)
        {
            return null;
        }

        var resultado = JsonSerializer.Deserialize<ResultadoDiagnostico>(diagnostico.Detalle, Json)!;
        var anterior = await _db.Diagnosticos
            .Where(d => d.Fecha < diagnostico.Fecha)
            .OrderByDescending(d => d.Fecha)
            .FirstOrDefaultAsync();
        return (diagnostico, resultado, anterior);
    }
}
