using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>Leer en contexto: textos reales en inglés y guardar sus palabras con la frase como ejemplo.</summary>
public class LecturasController : Controller
{
    private readonly LecturasService _lecturas;

    public LecturasController(LecturasService lecturas)
    {
        _lecturas = lecturas;
    }

    // Mis lecturas y el formulario para pegar una nueva.
    public async Task<IActionResult> Index()
    {
        return View(await _lecturas.ListarAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(string? titulo, string? texto)
    {
        var lectura = await _lecturas.CrearAsync(titulo, texto);
        if (lectura == null)
        {
            TempData["Mensaje"] = "Pega un texto en inglés para poder leerlo.";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Leer), new { id = lectura.Id });
    }

    // El texto con cada palabra tocable.
    public async Task<IActionResult> Leer(int id)
    {
        var lectura = await _lecturas.AbrirAsync(id);
        return lectura == null ? NotFound() : View(lectura);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrar(int id)
    {
        await _lecturas.BorrarAsync(id);
        TempData["Mensaje"] = "Borré la lectura. Las palabras que guardaste siguen en tu repaso.";
        return RedirectToAction(nameof(Index));
    }

    // Lo usa la página al tocar una palabra: su significado si ya existe en la app.
    public async Task<IActionResult> Significado(string palabra)
    {
        return Json(new { significado = await _lecturas.BuscarSignificadoAsync(palabra) });
    }

    // Lo usa la página para guardar una palabra en el repaso.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarPalabra(string? palabra, string? frase, string? significado)
    {
        var resultado = await _lecturas.GuardarPalabraAsync(palabra, frase, significado);
        return Json(new { guardada = resultado.Guardada, mensaje = resultado.Mensaje, significado = resultado.Significado });
    }
}
