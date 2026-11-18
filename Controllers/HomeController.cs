using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class HomeController : Controller
{
    private readonly RepasoService _repaso;

    public HomeController(RepasoService repaso)
    {
        _repaso = repaso;
    }

    public async Task<IActionResult> Index()
    {
        var resumen = await _repaso.ObtenerResumenAsync();
        return View(resumen);
    }

    // Pantalla para elegir tu meta diaria de ejercicios.
    public async Task<IActionResult> Meta()
    {
        ViewBag.MetaDiaria = await _repaso.ObtenerMetaDiariaAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Meta(int metaDiaria)
    {
        await _repaso.GuardarMetaDiariaAsync(metaDiaria);
        TempData["Mensaje"] = $"Listo: tu meta es de {await _repaso.ObtenerMetaDiariaAsync()} ejercicios al día.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Error()
    {
        return View();
    }
}
