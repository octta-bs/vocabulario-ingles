using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

public class RepasoService
{
    private readonly AppDbContext _db;
    private readonly IAlgoritmoRepaso _algoritmo;
    private readonly GeneradorEjercicios _generador;
    private readonly IRitmoPalabrasNuevas _ritmo;
    private readonly ZonaHorariaUsuario _zona;

    public RepasoService(AppDbContext db, IAlgoritmoRepaso algoritmo, GeneradorEjercicios generador,
        IRitmoPalabrasNuevas ritmo, ZonaHorariaUsuario zona)
    {
        _db = db;
        _algoritmo = algoritmo;
        _generador = generador;
        _ritmo = ritmo;
        _zona = zona;
    }

    /// <param name="modo">Si viene, solo cuenta las palabras de ese modo.</param>
    public async Task<int> ContarPendientesAsync(ModoApp? modo = null)
    {
        var ahora = DateTime.UtcNow;
        return await (await ProgresosDelModoAsync(modo)).CountAsync(p => p.ProximoRepaso <= ahora);
    }

    /// <summary>Los progresos de las palabras de un modo (o todos, si no se indica modo).</summary>
    private async Task<IQueryable<ProgresoPalabra>> ProgresosDelModoAsync(ModoApp? modo)
    {
        if (modo == null)
        {
            return _db.Progresos;
        }

        var ids = await Modos.CategoriasAsync(_db, modo.Value);
        return _db.Progresos.Where(p => ids.Contains(p.Palabra!.CategoriaId));
    }

    /// <summary>Las palabras de un modo (o todas, si no se indica modo).</summary>
    private async Task<IQueryable<Palabra>> PalabrasDelModoAsync(ModoApp? modo)
    {
        if (modo == null)
        {
            return _db.Palabras;
        }

        var ids = await Modos.CategoriasAsync(_db, modo.Value);
        return _db.Palabras.Where(p => ids.Contains(p.CategoriaId));
    }

    /// <summary>La palabra que más tiempo lleva sin repasarse entre las que ya tocan.</summary>
    public async Task<ProgresoPalabra?> SiguientePendienteAsync(ModoApp? modo = null)
    {
        var ahora = DateTime.UtcNow;
        return await (await ProgresosDelModoAsync(modo))
            .Include(p => p.Palabra)
            .Where(p => p.ProximoRepaso <= ahora)
            .OrderBy(p => p.UltimoRepaso)
            .FirstOrDefaultAsync();
    }

    /// <summary>El siguiente ejercicio del repaso (de un tipo elegido al azar), o null si no hay pendientes.</summary>
    public async Task<Ejercicio?> SiguienteEjercicioAsync(ModoApp? modo = null)
    {
        var progreso = await SiguientePendienteAsync(modo);
        return progreso?.Palabra == null ? null : _generador.Elegir(progreso.Palabra);
    }

    public async Task<RepasoResultado?> ResponderAsync(int palabraId, TipoEjercicio tipo, string? respuesta)
    {
        var progreso = await _db.Progresos
            .Include(p => p.Palabra)
            .FirstOrDefaultAsync(p => p.PalabraId == palabraId);

        if (progreso == null || progreso.Palabra == null)
        {
            return null;
        }

        // Acertar o fallar cuenta igual para el repaso, sea cual sea el tipo de ejercicio.
        var ejercicio = _generador.Crear(progreso.Palabra, tipo);
        var acerto = ComparadorRespuestas.SonIguales(respuesta, ejercicio.Respuesta);
        var ahora = DateTime.UtcNow;

        // Las últimas respuestas de esta palabra (la de ahora primero), para que el algoritmo
        // sepa si es una palabra que fallas seguido.
        var anteriores = await _db.Respuestas
            .Where(r => r.PalabraId == palabraId)
            .OrderByDescending(r => r.Fecha)
            .Select(r => r.Acerto)
            .Take(20)
            .ToListAsync();
        var ultimas = new List<bool> { acerto };
        ultimas.AddRange(anteriores);

        var resultado = _algoritmo.Calcular(progreso.Etapa, acerto, ahora, ultimas);
        _db.Respuestas.Add(new RegistroRespuesta
        {
            PalabraId = palabraId,
            Tipo = ejercicio.Tipo,
            Acerto = acerto,
            Fecha = ahora
        });

        progreso.Etapa = resultado.NuevaEtapa;
        progreso.ProximoRepaso = resultado.ProximoRepaso;
        progreso.UltimoRepaso = ahora;
        if (acerto)
        {
            progreso.Aciertos++;
        }
        else
        {
            progreso.Fallos++;
        }

        await _db.SaveChangesAsync();

        return new RepasoResultado
        {
            Acerto = acerto,
            RespuestaDada = (respuesta ?? "").Trim(),
            Palabra = progreso.Palabra,
            Ejercicio = ejercicio
        };
    }

    /// <summary>
    /// Califica una respuesta sin guardar nada: no cambia el repaso, el historial ni la meta.
    /// Se usa en "Practicar mis errores".
    /// </summary>
    public async Task<RepasoResultado?> EvaluarSinGuardarAsync(int palabraId, TipoEjercicio tipo, string? respuesta)
    {
        var palabra = await _db.Palabras.FirstOrDefaultAsync(p => p.Id == palabraId);
        if (palabra == null)
        {
            return null;
        }

        var ejercicio = _generador.Crear(palabra, tipo);
        return new RepasoResultado
        {
            Acerto = ComparadorRespuestas.SonIguales(respuesta, ejercicio.Respuesta),
            RespuestaDada = (respuesta ?? "").Trim(),
            Palabra = palabra,
            Ejercicio = ejercicio
        };
    }

    /// <summary>Cuántas palabras nuevas te tocan según tus aciertos recientes.</summary>
    public async Task<RitmoNuevas> CalcularRitmoAsync()
    {
        var ultimas = await _db.Respuestas
            .OrderByDescending(r => r.Fecha)
            .Select(r => r.Acerto)
            .Take(_ritmo.RespuestasQueUsa)
            .ToListAsync();

        return _ritmo.Calcular(ultimas);
    }

    public async Task<int> ContarSinEmpezarAsync(ModoApp? modo = null)
    {
        return await (await PalabrasDelModoAsync(modo)).CountAsync(p => p.Progreso == null);
    }

    /// <summary>Las categorías que todavía tienen palabras sin empezar, con cuántas les quedan.</summary>
    public async Task<List<CategoriaSinEmpezar>> CategoriasConNuevasAsync(ModoApp? modo = null)
    {
        var conteos = await (await PalabrasDelModoAsync(modo))
            .Where(p => p.Progreso == null)
            .GroupBy(p => p.CategoriaId)
            .Select(g => new { CategoriaId = g.Key, Cantidad = g.Count(), PrimeraPalabra = g.Min(p => p.Id) })
            .ToListAsync();

        // Mismo orden que AgregarNuevasAsync: por nivel y, dentro de cada nivel, como se cargaron.
        // Así la primera categoría es la que de verdad sigue "en orden".
        var categorias = await _db.Categorias.ToDictionaryAsync(c => c.Id);
        return conteos
            .OrderBy(c => categorias[c.CategoriaId].Nivel)
            .ThenBy(c => c.PrimeraPalabra)
            .Select(c => new CategoriaSinEmpezar(c.CategoriaId, categorias[c.CategoriaId].Nombre,
                categorias[c.CategoriaId].Nivel, c.Cantidad))
            .ToList();
    }

    /// <summary>
    /// Mete al repaso las siguientes palabras que todavía no se han empezado a aprender.
    /// La cantidad la decide la estrategia de ritmo según tus aciertos. Si eliges una categoría,
    /// solo salen de ella; si no, de nivel en nivel (Pre A1 primero). Con un modo, solo palabras de ese modo.
    /// </summary>
    public async Task<int> AgregarNuevasAsync(int? categoriaId = null, ModoApp? modo = null)
    {
        var cantidad = (await CalcularRitmoAsync()).Cantidad;
        if (cantidad <= 0)
        {
            return 0;
        }

        var ahora = DateTime.UtcNow;

        var nuevas = await (await PalabrasDelModoAsync(modo))
            .Where(p => p.Progreso == null)
            .Where(p => categoriaId == null || p.CategoriaId == categoriaId)
            // En orden: primero los niveles más básicos (Pre A1, A1…) y, dentro de cada uno, como se cargaron.
            .OrderBy(p => p.Categoria!.Nivel)
            .ThenBy(p => p.Id)
            .Take(cantidad)
            .ToListAsync();

        foreach (var palabra in nuevas)
        {
            _db.Progresos.Add(new ProgresoPalabra
            {
                PalabraId = palabra.Id,
                Etapa = 0,
                ProximoRepaso = ahora
            });
        }

        await _db.SaveChangesAsync();
        return nuevas.Count;
    }

    /// <summary>Tu meta diaria y cuántos ejercicios llevas hoy (aciertos y fallos cuentan igual).</summary>
    public async Task<MetaDelDia> ObtenerMetaHoyAsync()
    {
        var configuracion = await ObtenerConfiguracionAsync();
        var inicioDeHoy = _zona.InicioDeHoyUtc();
        var respondidas = await _db.Respuestas.CountAsync(r => r.Fecha >= inicioDeHoy);
        return new MetaDelDia(configuracion.MetaDiaria, respondidas);
    }

    public async Task<int> ObtenerMetaDiariaAsync()
    {
        return (await ObtenerConfiguracionAsync()).MetaDiaria;
    }

    public async Task GuardarMetaDiariaAsync(int meta)
    {
        var configuracion = await ObtenerConfiguracionAsync();
        configuracion.MetaDiaria = Math.Clamp(meta, Configuracion.MetaMinima, Configuracion.MetaMaxima);
        await _db.SaveChangesAsync();
    }

    /// <summary>La única fila de configuración; si todavía no existe, la crea con los valores por defecto.</summary>
    private async Task<Configuracion> ObtenerConfiguracionAsync()
    {
        var configuracion = await _db.Configuraciones.OrderBy(c => c.Id).FirstOrDefaultAsync();
        if (configuracion == null)
        {
            configuracion = new Configuracion();
            _db.Configuraciones.Add(configuracion);
            await _db.SaveChangesAsync();
        }

        return configuracion;
    }
}
