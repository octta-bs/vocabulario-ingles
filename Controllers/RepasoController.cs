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

    // Cookie donde se recuerda la última categoría elegida para las palabras nuevas.
    private const string CookieCategoria = "categoriaNuevas";

    // Muestra el siguiente ejercicio pendiente (o avisa que no hay).
    public async Task<IActionResult> Index()
    {
        var ejercicio = await _servicio.SiguienteEjercicioAsync();
        ViewBag.Pendientes = await _servicio.ContarPendientesAsync();
        ViewBag.Meta = await _servicio.ObtenerMetaHoyAsync();
        ViewBag.Ritmo = await _servicio.CalcularRitmoAsync();
        ViewBag.SinEmpezar = await _servicio.ContarSinEmpezarAsync();
        ViewBag.Categorias = await _servicio.CategoriasConNuevasAsync();
        ViewBag.CategoriaElegida = int.TryParse(Request.Cookies[CookieCategoria], out var elegida) ? elegida : (int?)null;
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
    public async Task<IActionResult> AgregarNuevas(int? categoriaId)
    {
        // Se recuerda la elección para la próxima vez ("Cualquiera" borra la cookie).
        if (categoriaId.HasValue)
        {
            Response.Cookies.Append(CookieCategoria, categoriaId.Value.ToString(),
                new CookieOptions { MaxAge = TimeSpan.FromDays(365), HttpOnly = true, IsEssential = true });
        }
        else
        {
            Response.Cookies.Delete(CookieCategoria);
        }

        var ritmo = await _servicio.CalcularRitmoAsync();
        var cantidad = await _servicio.AgregarNuevasAsync(categoriaId);
        TempData["Mensaje"] = cantidad > 0
            ? $"Se agregaron {cantidad} palabras nuevas a tu repaso."
            : ritmo.Cantidad == 0
                ? "Por ahora no te tocan palabras nuevas: primero refuerza las que ya tienes."
                : "Ya agregaste todas las palabras disponibles.";

        return RedirectToAction(nameof(Index));
    }
}
