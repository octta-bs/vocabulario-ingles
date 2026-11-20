using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Práctica en voz alta: muestra una frase al azar y el navegador reconoce lo que dices.
/// Es solo práctica: no toca el repaso, la meta ni la base de datos.
/// </summary>
public class PracticaController : Controller
{
    private readonly AppDbContext _db;

    public PracticaController(AppDbContext db)
    {
        _db = db;
    }

    /// <param name="categoriaId">Si viene, solo salen frases de esa categoría.</param>
    /// <param name="anterior">La frase que acabas de practicar, para no repetirla enseguida.</param>
    public async Task<IActionResult> Index(int? categoriaId, int? anterior)
    {
        return View(await ElegirFraseAsync(categoriaId, anterior));
    }

    /// <summary>
    /// Shadowing en 3 pasos: escuchas la frase, la repites al mismo tiempo que el audio (lento y normal)
    /// y al final la dices tú solo; solo ese último paso se califica.
    /// </summary>
    public async Task<IActionResult> Shadowing(int? categoriaId, int? anterior)
    {
        return View(await ElegirFraseAsync(categoriaId, anterior));
    }

    /// <summary>Una frase al azar (de la categoría elegida, si hay) y la lista de categorías para el filtro.</summary>
    private async Task<Palabra?> ElegirFraseAsync(int? categoriaId, int? anterior)
    {
        var frases = _db.Palabras.Where(p => p.Tipo == TipoPalabra.Frase);

        ViewBag.Categorias = await frases
            .Select(p => p.Categoria!)
            .Distinct()
            // De la más fácil a la más difícil, igual que las palabras nuevas.
            .OrderBy(c => c.Nivel)
            .ThenBy(c => c.Nombre)
            .ToListAsync();
        ViewBag.CategoriaId = categoriaId;

        if (categoriaId.HasValue)
        {
            frases = frases.Where(p => p.CategoriaId == categoriaId.Value);
        }

        var candidatas = await frases.Select(p => p.Id).ToListAsync();
        if (candidatas.Count > 1 && anterior.HasValue)
        {
            candidatas.Remove(anterior.Value);
        }

        if (candidatas.Count == 0)
        {
            return null;
        }

        var id = candidatas[Random.Shared.Next(candidatas.Count)];
        return await _db.Palabras.Include(p => p.Categoria).FirstAsync(p => p.Id == id);
    }
}
