using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;

namespace VocabularioIngles.Controllers;

public class PalabrasController : Controller
{
    private readonly AppDbContext _db;

    public PalabrasController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = await _db.Categorias.OrderBy(c => c.Nivel).ThenBy(c => c.Id).ToListAsync();
        ViewBag.CategoriaId = categoriaId;

        var consulta = _db.Palabras.Include(p => p.Categoria).AsQueryable();
        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
        }

        var palabras = await consulta.OrderBy(p => p.Ingles).ToListAsync();
        return View(palabras);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var palabra = await _db.Palabras
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (palabra == null)
        {
            return NotFound();
        }

        return View(palabra);
    }
}
