using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>Cómo vas en una meta. Avance va de 0 a 1; es null en las prácticas sugeridas, que no se miden.</summary>
public record MetaEstado(RutaAprendizaje.Meta Meta, int Etapa, int Indice, double? Avance, string Detalle)
{
    public bool Medible => Avance.HasValue;
    public bool Hecha => Avance >= 1;
}

/// <summary>Cómo vas en una etapa. Se completa con el 80 % de sus metas medibles, o si el diagnóstico ya te pone arriba.</summary>
public record EtapaEstado(RutaAprendizaje.Etapa Etapa, List<MetaEstado> Metas, double Avance, bool PorDiagnostico)
{
    public bool Completa => PorDiagnostico || Avance >= RutaService.ParaCompletar;
    public int Porcentaje => (int)Math.Round(Avance * 100);
}

/// <summary>Toda la ruta: tus etapas, en cuál vas y la siguiente meta.</summary>
public record RutaEstado(List<EtapaEstado> Etapas, EtapaEstado Actual, MetaEstado? Siguiente);

/// <summary>A dónde lleva una meta, y si antes hay que cambiar de modo o elegir una categoría de palabras nuevas.</summary>
public record DestinoMeta(string Controlador, string Accion, string? Id = null, ModoApp? Modo = null, int? CategoriaNuevas = null);

/// <summary>
/// Calcula tu avance en la ruta de aprendizaje con lo que la app ya guarda: palabras y frases aprendidas (acertadas
/// en días distintos), temas de gramática, sonidos dominados, lecturas y respuestas escritas. No guarda nada nuevo.
/// </summary>
public class RutaService
{
    // Una palabra cuenta como aprendida desde esta etapa del repaso (la acertaste al menos dos veces, en días distintos).
    public const int EtapaAprendida = 2;

    // Una etapa de la ruta se completa con este avance.
    public const double ParaCompletar = 0.8;

    private readonly AppDbContext _db;
    private readonly SonidosService _sonidos;

    public RutaService(AppDbContext db, SonidosService sonidos)
    {
        _db = db;
        _sonidos = sonidos;
    }

    public async Task<RutaEstado> CalcularAsync()
    {
        // Unas cientos de filas: se agrupan en memoria.
        var palabras = await _db.Palabras
            .Select(p => new { Categoria = p.Categoria!.Nombre, Etapa = p.Progreso == null ? (int?)null : p.Progreso.Etapa })
            .ToListAsync();
        var porCategoria = palabras
            .GroupBy(p => p.Categoria)
            .ToDictionary(g => g.Key, g => (Total: g.Count(), Aprendidas: g.Count(p => p.Etapa >= EtapaAprendida)));
        var sonidos = (await _sonidos.ListarAsync()).ToDictionary(s => s.Sonido.Clave);
        var lecturas = await _db.Lecturas.CountAsync();
        var respuestasPropias = await _db.RespuestasPropias.CountAsync();
        var diagnostico = await _db.Diagnosticos.OrderByDescending(d => d.Fecha).FirstOrDefaultAsync();

        // El diagnóstico te coloca: las etapas hasta tu nivel más bajo entre las áreas cuentan como hechas. La gramática
        // empieza en A2; sin nivel en ella, cuenta como A1.
        NivelMcer? nivelDiagnostico = diagnostico is { NivelVocabulario: { } v, NivelEscucha: { } e }
            ? new[] { v, e, diagnostico.NivelGramatica ?? NivelMcer.A1 }.Min()
            : null;

        (double? Avance, string Detalle) Medir(RutaAprendizaje.Meta meta)
        {
            switch (meta.Tipo)
            {
                case TipoMeta.Categoria:
                case TipoMeta.TemaGramatica:
                {
                    var nombre = meta.Tipo == TipoMeta.Categoria ? meta.Clave : TemasGramatica.Buscar(meta.Clave)!.Categoria;
                    if (!porCategoria.TryGetValue(nombre, out var c) || c.Total == 0)
                    {
                        return (0, "sin contenido");
                    }

                    return ((double)c.Aprendidas / c.Total, $"{c.Aprendidas} de {c.Total} aprendidas");
                }
                case TipoMeta.Sonido:
                {
                    var s = sonidos[meta.Clave];
                    return s.Dominado ? (1, "dominado")
                        : s.Empezado ? (Math.Min(0.9, s.Aciertos / 8.0), $"{s.Aciertos} de {s.Respondidas} bien")
                        : (0, "sin empezar");
                }
                case TipoMeta.Lecturas:
                {
                    var meta2 = int.Parse(meta.Clave);
                    return (Math.Min(1, (double)lecturas / meta2), $"{Math.Min(lecturas, meta2)} de {meta2} textos");
                }
                case TipoMeta.MisRespuestas:
                {
                    var meta2 = int.Parse(meta.Clave);
                    return (Math.Min(1, (double)respuestasPropias / meta2), $"{Math.Min(respuestasPropias, meta2)} de {meta2} escritas");
                }
                default:
                    return (null, "práctica sugerida");
            }
        }

        var etapas = RutaAprendizaje.Etapas
            .Select(etapa =>
            {
                var metas = etapa.Metas
                    .Select((meta, i) =>
                    {
                        var (avance, detalle) = Medir(meta);
                        return new MetaEstado(meta, etapa.Numero, i, avance, detalle);
                    })
                    .ToList();
                var medibles = metas.Where(m => m.Medible).ToList();
                var avanceEtapa = medibles.Count == 0 ? 0 : medibles.Average(m => m.Avance!.Value);
                return new EtapaEstado(etapa, metas, avanceEtapa, nivelDiagnostico >= etapa.Nivel);
            })
            .ToList();

        var actual = etapas.FirstOrDefault(e => !e.Completa) ?? etapas[^1];
        var siguiente = actual.Metas.FirstOrDefault(m => m.Medible && !m.Hecha)
                        ?? actual.Metas.FirstOrDefault(m => !m.Medible);
        return new RutaEstado(etapas, actual, siguiente);
    }

    /// <summary>A dónde lleva la meta número <paramref name="indice"/> de la etapa <paramref name="etapa"/>.</summary>
    public async Task<DestinoMeta?> DestinoAsync(int etapa, int indice)
    {
        var meta = RutaAprendizaje.Etapas.FirstOrDefault(e => e.Numero == etapa)?.Metas.ElementAtOrDefault(indice);
        switch (meta?.Tipo)
        {
            case null:
                return null;
            case TipoMeta.Categoria:
            {
                var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Nombre == meta.Clave);
                if (categoria == null)
                {
                    return null;
                }

                // Si le quedan palabras sin empezar, a palabras nuevas con esa categoría elegida; si no, a repasarlas.
                var modo = Modos.De(categoria.Nombre);
                var sinEmpezar = await _db.Palabras.AnyAsync(p => p.CategoriaId == categoria.Id && p.Progreso == null);
                return sinEmpezar
                    ? new DestinoMeta("Repaso", "Nuevas", Modo: modo, CategoriaNuevas: categoria.Id)
                    : new DestinoMeta("Repaso", "Index", Modo: modo);
            }
            case TipoMeta.TemaGramatica:
                return new DestinoMeta("Gramatica", "Tema", meta.Clave);
            case TipoMeta.Sonido:
                return new DestinoMeta("Sonidos", "Ver", meta.Clave);
            case TipoMeta.Lecturas:
                return new DestinoMeta("Lecturas", "Index", Modo: ModoApp.Trabajo);
            case TipoMeta.MisRespuestas:
                return new DestinoMeta("MisRespuestas", "Index", Modo: ModoApp.Trabajo);
            default:
                return new DestinoMeta(meta.Controlador!, meta.Accion!, meta.Id, ModoApp.Trabajo);
        }
    }
}
