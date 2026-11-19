using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>Un tema de gramática con cuántas de sus frases ya están en tu repaso.</summary>
public record TemaConEstado(TemasGramatica.Tema Tema, int Total, int EnRepaso)
{
    public bool Empezado => EnRepaso > 0;
    public bool Completo => Total > 0 && EnRepaso == Total;
}

/// <summary>
/// Los temas de gramática: cuáles empezaste y meter al repaso todas las frases de un tema cuando eliges practicarlo.
/// Las frases se cargan al arrancar (<see cref="SeedData"/>) como palabras de tipo gramática.
/// </summary>
public class GramaticaService
{
    private readonly AppDbContext _db;
    private readonly TimeProvider _reloj;

    public GramaticaService(AppDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public async Task<List<TemaConEstado>> ListarAsync()
    {
        var conteos = await _db.Palabras
            .Where(p => p.Tipo == TipoPalabra.Gramatica)
            .GroupBy(p => p.Categoria!.Nombre)
            .Select(g => new { Nombre = g.Key, Total = g.Count(), EnRepaso = g.Count(p => p.Progreso != null) })
            .ToDictionaryAsync(g => g.Nombre);

        return TemasGramatica.Todos
            .Select(t => conteos.TryGetValue(t.Categoria, out var c)
                ? new TemaConEstado(t, c.Total, c.EnRepaso)
                : new TemaConEstado(t, 0, 0))
            .ToList();
    }

    public async Task<TemaConEstado?> ObtenerAsync(string? clave)
    {
        var tema = TemasGramatica.Buscar(clave);
        return tema == null ? null : (await ListarAsync()).First(t => t.Tema.Clave == tema.Clave);
    }

    /// <summary>
    /// Mete al repaso todas las frases del tema que todavía no estén, para repasarlas desde ahora.
    /// Regresa cuántas entraron (0 si el tema no existe o ya estaban todas).
    /// </summary>
    public async Task<int> PracticarAsync(string? clave)
    {
        var tema = TemasGramatica.Buscar(clave);
        if (tema == null)
        {
            return 0;
        }

        var nuevas = await _db.Palabras
            .Where(p => p.Tipo == TipoPalabra.Gramatica && p.Categoria!.Nombre == tema.Categoria && p.Progreso == null)
            .OrderBy(p => p.Id)
            .ToListAsync();

        var ahora = _reloj.GetUtcNow().UtcDateTime;
        foreach (var palabra in nuevas)
        {
            _db.Progresos.Add(new ProgresoPalabra { PalabraId = palabra.Id, Etapa = 0, ProximoRepaso = ahora });
        }

        await _db.SaveChangesAsync();
        return nuevas.Count;
    }
}
