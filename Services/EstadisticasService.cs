using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>
/// Calcula tu progreso a partir del historial de respuestas: racha de días cumpliendo la meta,
/// ejercicios por día y las palabras que más fallas. Solo lee; no cambia nada.
/// </summary>
public class EstadisticasService
{
    public const int DiasEnGrafica = 14;

    private readonly AppDbContext _db;
    private readonly ZonaHorariaUsuario _zona;

    public EstadisticasService(AppDbContext db, ZonaHorariaUsuario zona)
    {
        _db = db;
        _zona = zona;
    }

    public async Task<ResumenProgreso> ObtenerAsync()
    {
        var meta = await _db.Configuraciones.OrderBy(c => c.Id).Select(c => (int?)c.MetaDiaria).FirstOrDefaultAsync()
                   ?? Configuracion.MetaPorDefecto;

        // Una sola persona usa la app, así que el historial completo cabe sin problema en memoria.
        var respuestas = await _db.Respuestas.Select(r => new { r.Fecha, r.Acerto }).ToListAsync();
        var porDia = respuestas
            .GroupBy(r => _zona.DiaLocal(r.Fecha))
            .ToDictionary(g => g.Key, g => new DiaDeProgreso(g.Key, g.Count(), g.Count(r => r.Acerto)));

        var hoy = _zona.Hoy;
        bool Cumplido(DateOnly dia) => porDia.TryGetValue(dia, out var d) && d.Ejercicios >= meta;

        var hoyCumplida = Cumplido(hoy);
        var racha = 0;
        for (var dia = hoyCumplida ? hoy : hoy.AddDays(-1); Cumplido(dia); dia = dia.AddDays(-1))
        {
            racha++;
        }

        var mejor = 0;
        var seguidos = 0;
        DateOnly? anterior = null;
        foreach (var dia in porDia.Keys.Where(Cumplido).Order())
        {
            seguidos = anterior == dia.AddDays(-1) ? seguidos + 1 : 1;
            mejor = Math.Max(mejor, seguidos);
            anterior = dia;
        }

        var ultimos = Enumerable.Range(0, DiasEnGrafica)
            .Select(i => hoy.AddDays(i - DiasEnGrafica + 1))
            .Select(dia => porDia.TryGetValue(dia, out var d) ? d : new DiaDeProgreso(dia, 0, 0))
            .ToList();

        return new ResumenProgreso
        {
            Meta = meta,
            RachaActual = racha,
            MejorRacha = mejor,
            HoyCumplida = hoyCumplida,
            TotalRespuestas = respuestas.Count,
            PorcentajeTotal = respuestas.Count == 0 ? null : respuestas.Count(r => r.Acerto) * 100 / respuestas.Count,
            UltimosDias = ultimos,
            MasFalladas = await MasFalladasAsync(10)
        };
    }

    /// <summary>Las palabras que más has fallado (primero las de más fallos).</summary>
    public async Task<List<PalabraDificil>> MasFalladasAsync(int cantidad)
    {
        var conteos = await _db.Respuestas
            .GroupBy(r => r.PalabraId)
            .Select(g => new
            {
                PalabraId = g.Key,
                Fallos = g.Count(r => !r.Acerto),
                Aciertos = g.Count(r => r.Acerto)
            })
            .Where(c => c.Fallos > 0)
            .OrderByDescending(c => c.Fallos)
            .ThenBy(c => c.Aciertos)
            .ThenBy(c => c.PalabraId)
            .Take(cantidad)
            .ToListAsync();

        var ids = conteos.Select(c => c.PalabraId).ToList();
        var palabras = await _db.Palabras.Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        return conteos.Select(c => new PalabraDificil(palabras[c.PalabraId], c.Fallos, c.Aciertos)).ToList();
    }
}
