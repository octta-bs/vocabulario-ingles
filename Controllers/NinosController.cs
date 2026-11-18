using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Modo infantil: dibujos grandes, audio y tocar la respuesta (sin escribir). Tiene su propio
/// progreso, separado del repaso del adulto.
/// </summary>
public class NinosController : Controller
{
    private readonly JuegoInfantilService _juego;

    public NinosController(JuegoInfantilService juego)
    {
        _juego = juego;
    }

    // Elegir categoría.
    public async Task<IActionResult> Index()
    {
        ViewBag.Nombre = await _juego.ObtenerNombreAsync();
        return View(await _juego.CategoriasAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nombre(string? nombre)
    {
        await _juego.GuardarNombreAsync(nombre);
        return RedirectToAction(nameof(Index));
    }

    // Tarjetas para conocer las palabras: tocar un dibujo lo dice en inglés.
    public async Task<IActionResult> Aprende(int id)
    {
        var categoria = await _juego.CategoriaAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        ViewBag.Categoria = categoria;
        return View(await _juego.PalabrasAsync(id));
    }

    // "Escucha y toca": 8 rondas con 3 dibujos cada una.
    public async Task<IActionResult> Juego(int id)
    {
        var categoria = await _juego.CategoriaAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        ViewBag.Categoria = categoria;
        ViewBag.Nombre = await _juego.ObtenerNombreAsync();
        return View(await _juego.ArmarJuegoAsync(id, Random.Shared));
    }

    // Guarda cada respuesta del juego (lo llama el JavaScript de la página).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(int palabraId, bool acerto)
    {
        return await _juego.RegistrarAsync(palabraId, acerto) ? NoContent() : BadRequest();
    }
}
