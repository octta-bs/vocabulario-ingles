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

    public IActionResult Error()
    {
        return View();
    }
}
