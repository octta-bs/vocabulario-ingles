using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class RepasoController : Controller
{
    private readonly RepasoService _servicio;

    public RepasoController(RepasoService servicio)
    {
        _servicio = servicio;
    }

    // Muestra la siguiente palabra pendiente (o avisa que no hay).
    public async Task<IActionResult> Index()
    {
        var pendiente = await _servicio.SiguientePendienteAsync();
        ViewBag.Pendientes = await _servicio.ContarPendientesAsync();
        return View(pendiente);
    }

    // Recibe lo que escribiste, lo compara y muestra el resultado.
    [HttpPost]
    public async Task<IActionResult> Responder(int palabraId, string? respuesta)
    {
        var resultado = await _servicio.ResponderAsync(palabraId, respuesta);
        if (resultado == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View("Resultado", resultado);
    }

    // Mete 5 palabras nuevas al repaso.
    [HttpPost]
    public async Task<IActionResult> AgregarNuevas()
    {
        var cantidad = await _servicio.AgregarNuevasAsync(5);
        TempData["Mensaje"] = cantidad > 0
            ? $"Se agregaron {cantidad} palabras nuevas a tu repaso."
            : "Ya agregaste todas las palabras disponibles.";

        return RedirectToAction(nameof(Index));
    }
}
