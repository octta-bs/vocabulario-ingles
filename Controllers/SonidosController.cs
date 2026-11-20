using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Sonidos difíciles: los sonidos del inglés que el español no tiene, con pares mínimos (ship / sheep).
/// "Escucha y elige" entrena el oído; "Dila tú" usa el reconocimiento de voz para ver qué palabra se entendió.
/// </summary>
public class SonidosController : Controller
{
    private readonly SonidosService _sonidos;

    public SonidosController(SonidosService sonidos)
    {
        _sonidos = sonidos;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _sonidos.ListarAsync());
    }

    public async Task<IActionResult> Ver(string id)
    {
        var sonido = await _sonidos.ObtenerAsync(id);
        return sonido == null ? NotFound() : View(sonido);
    }

    // Lo usa la página después de cada respuesta.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(string? sonido, EjercicioSonido ejercicio, bool acerto)
    {
        var estado = await _sonidos.GuardarAsync(sonido, ejercicio, acerto);
        return estado == null
            ? NotFound()
            : Json(new { respondidas = estado.Respondidas, aciertos = estado.Aciertos, dominado = estado.Dominado });
    }
}
