using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class RepasoController : Controller
{
    private readonly RepasoService _servicio;

    public RepasoController(RepasoService servicio)
    {
        _servicio = servicio;
    }

    // Muestra el siguiente ejercicio pendiente (o avisa que no hay).
    public async Task<IActionResult> Index()
    {
        var ejercicio = await _servicio.SiguienteEjercicioAsync();
        ViewBag.Pendientes = await _servicio.ContarPendientesAsync();
        ViewBag.Meta = await _servicio.ObtenerMetaHoyAsync();
        ViewBag.Ritmo = await _servicio.CalcularRitmoAsync();
        ViewBag.SinEmpezar = await _servicio.ContarSinEmpezarAsync();
        return View(ejercicio);
    }

    // Recibe lo que escribiste, lo compara y muestra el resultado.
    [HttpPost]
    public async Task<IActionResult> Responder(int palabraId, TipoEjercicio tipo, string? respuesta)
    {
        var resultado = await _servicio.ResponderAsync(palabraId, tipo, respuesta);
        if (resultado == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View("Resultado", resultado);
    }

    // Mete al repaso las palabras nuevas que te tocan según tus aciertos.
    [HttpPost]
    public async Task<IActionResult> AgregarNuevas()
    {
        var ritmo = await _servicio.CalcularRitmoAsync();
        var cantidad = await _servicio.AgregarNuevasAsync();
        TempData["Mensaje"] = cantidad > 0
            ? $"Se agregaron {cantidad} palabras nuevas a tu repaso."
            : ritmo.Cantidad == 0
                ? "Por ahora no te tocan palabras nuevas: primero refuerza las que ya tienes."
                : "Ya agregaste todas las palabras disponibles.";

        return RedirectToAction(nameof(Index));
    }
}
