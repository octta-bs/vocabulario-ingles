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
    private readonly TimeZoneInfo _zonaHoraria;

    public RepasoService(AppDbContext db, IAlgoritmoRepaso algoritmo, GeneradorEjercicios generador,
        IRitmoPalabrasNuevas ritmo, IConfiguration configuracion)
    {
        _db = db;
        _algoritmo = algoritmo;
        _generador = generador;
        _ritmo = ritmo;
        _zonaHoraria = BuscarZonaHoraria(configuracion["ZonaHoraria"]);
    }

    public Task<int> ContarPendientesAsync()
    {
        var ahora = DateTime.UtcNow;
        return _db.Progresos.CountAsync(p => p.ProximoRepaso <= ahora);
    }

    /// <summary>La palabra que más tiempo lleva sin repasarse entre las que ya tocan.</summary>
    public Task<ProgresoPalabra?> SiguientePendienteAsync()
    {
        var ahora = DateTime.UtcNow;
        return _db.Progresos
            .Include(p => p.Palabra)
            .Where(p => p.ProximoRepaso <= ahora)
            .OrderBy(p => p.UltimoRepaso)
            .FirstOrDefaultAsync();
    }

    /// <summary>El siguiente ejercicio del repaso (de un tipo elegido al azar), o null si no hay pendientes.</summary>
    public async Task<Ejercicio?> SiguienteEjercicioAsync()
    {
        var progreso = await SiguientePendienteAsync();
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

    public Task<int> ContarSinEmpezarAsync()
    {
        return _db.Palabras.CountAsync(p => p.Progreso == null);
    }

    /// <summary>Las categorías que todavía tienen palabras sin empezar, con cuántas les quedan.</summary>
    public async Task<List<CategoriaSinEmpezar>> CategoriasConNuevasAsync()
    {
        var conteos = await _db.Palabras
            .Where(p => p.Progreso == null)
            .GroupBy(p => p.CategoriaId)
            .Select(g => new { CategoriaId = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        var categorias = await _db.Categorias.ToDictionaryAsync(c => c.Id, c => c.Nombre);
        return conteos
            .Select(c => new CategoriaSinEmpezar(c.CategoriaId, categorias[c.CategoriaId], c.Cantidad))
            .OrderBy(c => c.Nombre)
            .ToList();
    }

    /// <summary>
    /// Mete al repaso las siguientes palabras que todavía no se han empezado a aprender.
    /// La cantidad la decide la estrategia de ritmo según tus aciertos. Si eliges una categoría,
    /// solo salen de ella; si no, en el orden en que se cargaron.
    /// </summary>
    public async Task<int> AgregarNuevasAsync(int? categoriaId = null)
    {
        var cantidad = (await CalcularRitmoAsync()).Cantidad;
        if (cantidad <= 0)
        {
            return 0;
        }

        var ahora = DateTime.UtcNow;

        var nuevas = await _db.Palabras
            .Where(p => p.Progreso == null)
            .Where(p => categoriaId == null || p.CategoriaId == categoriaId)
            .OrderBy(p => p.Id)
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

    public async Task<ResumenViewModel> ObtenerResumenAsync()
    {
        var ahora = DateTime.UtcNow;

        return new ResumenViewModel
        {
            TotalPalabras = await _db.Palabras.CountAsync(),
            EnRepaso = await _db.Progresos.CountAsync(),
            Pendientes = await _db.Progresos.CountAsync(p => p.ProximoRepaso <= ahora),
            Dominadas = await _db.Progresos.CountAsync(p => p.Etapa >= IntervalosFijos.EtapaMaxima),
            Meta = await ObtenerMetaHoyAsync()
        };
    }

    /// <summary>Tu meta diaria y cuántos ejercicios llevas hoy (aciertos y fallos cuentan igual).</summary>
    public async Task<MetaDelDia> ObtenerMetaHoyAsync()
    {
        var configuracion = await ObtenerConfiguracionAsync();
        var inicioDeHoy = InicioDeHoyUtc();
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

    /// <summary>La medianoche de hoy en tu zona horaria, convertida a UTC (así se guardan las fechas).</summary>
    private DateTime InicioDeHoyUtc()
    {
        var ahoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _zonaHoraria);
        return TimeZoneInfo.ConvertTimeToUtc(ahoraLocal.Date, _zonaHoraria);
    }

    private static TimeZoneInfo BuscarZonaHoraria(string? id)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(id) ? "America/Mexico_City" : id);
        }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.Local;
        }
    }
}
