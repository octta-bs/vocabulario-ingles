using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

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
        // Solo las categorías del modo actual (Principiante o Trabajo).
        var modo = Modos.Leer(Request);
        var delModo = modo == null ? null : await Modos.CategoriasAsync(_db, modo.Value);
        var categorias = _db.Categorias.AsQueryable();
        var consulta = _db.Palabras.Include(p => p.Categoria).AsQueryable();
        if (delModo != null)
        {
            categorias = categorias.Where(c => delModo.Contains(c.Id));
            consulta = consulta.Where(p => delModo.Contains(p.CategoriaId));
        }

        ViewBag.Categorias = await categorias.OrderBy(c => c.Nivel).ThenBy(c => c.Id).ToListAsync();
        ViewBag.CategoriaId = categoriaId;
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
