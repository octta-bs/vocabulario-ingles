using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>Un sonido con tu avance: aciertos en tus últimas respuestas y si ya lo dominas.</summary>
/// <param name="Numero">Su lugar en el orden de estudio (1 = el primero).</param>
public record SonidoConEstado(SonidosDificiles.Sonido Sonido, int Numero, int Respondidas, int Aciertos)
{
    public bool Empezado => Respondidas > 0;

    // Lo dominas si de tus últimas respuestas (al menos 10) aciertas 8 de cada 10.
    public bool Dominado => Respondidas >= SonidosService.Ventana
                            && Aciertos >= SonidosService.Ventana * SonidosService.ProporcionDominado;
}

/// <summary>Guarda tus respuestas de "Sonidos difíciles" y calcula tu avance en cada sonido.</summary>
public class SonidosService
{
    // Cuántas respuestas recientes se miran por sonido, y qué parte hay que acertar para dominarlo.
    public const int Ventana = 10;
    public const double ProporcionDominado = 0.8;

    private readonly AppDbContext _db;
    private readonly TimeProvider _reloj;

    public SonidosService(AppDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public async Task<List<SonidoConEstado>> ListarAsync()
    {
        var respuestas = await _db.RespuestasSonido
            .OrderByDescending(r => r.Fecha)
            .ThenByDescending(r => r.Id)
            .Select(r => new { r.Sonido, r.Acerto })
            .ToListAsync();
        var porSonido = respuestas.GroupBy(r => r.Sonido)
            .ToDictionary(g => g.Key, g => g.Take(Ventana).ToList());

        return SonidosDificiles.Todos
            .Select((s, i) => porSonido.TryGetValue(s.Clave, out var ultimas)
                ? new SonidoConEstado(s, i + 1, ultimas.Count, ultimas.Count(r => r.Acerto))
                : new SonidoConEstado(s, i + 1, 0, 0))
            .ToList();
    }

    public async Task<SonidoConEstado?> ObtenerAsync(string? clave)
    {
        return SonidosDificiles.Buscar(clave) is { } sonido
            ? (await ListarAsync()).First(s => s.Sonido.Clave == sonido.Clave)
            : null;
    }

    /// <summary>El siguiente sonido que conviene practicar: el primero que todavía no dominas.</summary>
    public static SonidoConEstado? Siguiente(IEnumerable<SonidoConEstado> sonidos) => sonidos.FirstOrDefault(s => !s.Dominado);

    /// <summary>Guarda una respuesta. Regresa el avance actualizado, o null si el sonido no existe.</summary>
    public async Task<SonidoConEstado?> GuardarAsync(string? clave, EjercicioSonido ejercicio, bool acerto)
    {
        if (SonidosDificiles.Buscar(clave) is not { } sonido)
        {
            return null;
        }

        _db.RespuestasSonido.Add(new RespuestaSonido
        {
            Sonido = sonido.Clave,
            Ejercicio = ejercicio,
            Acerto = acerto,
            Fecha = _reloj.GetUtcNow().UtcDateTime
        });
        await _db.SaveChangesAsync();
        return await ObtenerAsync(sonido.Clave);
    }
}
