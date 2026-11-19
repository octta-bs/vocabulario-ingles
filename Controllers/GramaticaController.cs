using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Gramática: lees la regla de un tema y, si quieres, metes sus frases a tu repaso con "Practicar este tema".
/// Las frases son del modo Trabajo, así que se repasan en ese modo.
/// </summary>
public class GramaticaController : Controller
{
    private readonly GramaticaService _gramatica;

    public GramaticaController(GramaticaService gramatica)
    {
        _gramatica = gramatica;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _gramatica.ListarAsync());
    }

    public async Task<IActionResult> Tema(string id)
    {
        var tema = await _gramatica.ObtenerAsync(id);
        return tema == null ? NotFound() : View(tema);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Practicar(string id)
    {
        var tema = await _gramatica.ObtenerAsync(id);
        if (tema == null)
        {
            return NotFound();
        }

        var agregadas = await _gramatica.PracticarAsync(id);
        // Las frases de gramática son del modo Trabajo: se repasan ahí.
        Modos.Guardar(Response, ModoApp.Trabajo);
        TempData["Mensaje"] = agregadas == 0
            ? "Las frases de este tema ya estaban en tu repaso."
            : $"Agregué {agregadas} frases de \"{tema.Tema.Titulo}\" a tu repaso. Ya puedes practicarlas.";
        return RedirectToAction(nameof(Tema), new { id });
    }
}
