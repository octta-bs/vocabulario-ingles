using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// La ruta de aprendizaje: 5 etapas de cero a intermedio alto, con tu avance en cada meta. Guía sin bloquear:
/// cualquier cosa se puede abrir cuando quieras.
/// </summary>
public class RutaController : Controller
{
    private readonly RutaService _ruta;

    public RutaController(RutaService ruta)
    {
        _ruta = ruta;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _ruta.CalcularAsync());
    }

    /// <summary>Lleva a donde se practica una meta, cambiando antes de modo (y de categoría) si hace falta.</summary>
    public async Task<IActionResult> Ir(int etapa, int meta)
    {
        var destino = await _ruta.DestinoAsync(etapa, meta);
        if (destino == null)
        {
            return NotFound();
        }

        if (destino.Modo is { } modo)
        {
            Modos.Guardar(Response, modo);
        }

        if (destino.CategoriaNuevas is { } categoria)
        {
            Response.Cookies.Append(RepasoController.CookieCategoria, categoria.ToString(),
                new CookieOptions { MaxAge = TimeSpan.FromDays(365), HttpOnly = true, IsEssential = true });
        }

        return RedirectToAction(destino.Accion, destino.Controlador, destino.Id == null ? null : new { id = destino.Id });
    }
}
