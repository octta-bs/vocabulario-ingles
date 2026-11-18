using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Tus propias respuestas a las preguntas del simulacro. Se guardan en tu base de datos, no en el
/// repositorio, así que puedes escribir tus datos reales.
/// </summary>
public class MisRespuestasController : Controller
{
    private readonly RespuestasPropiasService _servicio;

    public MisRespuestasController(RespuestasPropiasService servicio)
    {
        _servicio = servicio;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _servicio.ObtenerAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Dictionary<string, string?> respuestas)
    {
        await _servicio.GuardarAsync(respuestas);
        TempData["Mensaje"] = "Listo: se guardaron tus respuestas.";
        return RedirectToAction(nameof(Index));
    }
}
