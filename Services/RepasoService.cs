using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

public class RepasoService
{
    private readonly AppDbContext _db;
    private readonly IAlgoritmoRepaso _algoritmo;
    private readonly GeneradorEjercicios _generador;

    public RepasoService(AppDbContext db, IAlgoritmoRepaso algoritmo, GeneradorEjercicios generador)
    {
        _db = db;
        _algoritmo = algoritmo;
        _generador = generador;
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
        var acerto = Normalizar(respuesta) == Normalizar(ejercicio.Respuesta);
        var ahora = DateTime.UtcNow;
        var resultado = _algoritmo.Calcular(progreso.Etapa, acerto, ahora);

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

    /// <summary>Mete al repaso las siguientes palabras que todavía no se han empezado a aprender.</summary>
    public async Task<int> AgregarNuevasAsync(int cantidad)
    {
        var ahora = DateTime.UtcNow;

        var nuevas = await _db.Palabras
            .Where(p => p.Progreso == null)
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
            Dominadas = await _db.Progresos.CountAsync(p => p.Etapa >= IntervalosFijos.EtapaMaxima)
        };
    }

    private static string Normalizar(string? texto)
    {
        return (texto ?? "").Trim().ToLowerInvariant();
    }
}
