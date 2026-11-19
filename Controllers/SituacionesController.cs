using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Data;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Situaciones guiadas: la app dice su parte de un diálogo de trabajo y tú respondes en voz alta
/// con una de las respuestas modelo. Es solo práctica: no se guarda nada.
/// </summary>
public class SituacionesController : Controller
{
    public IActionResult Index() => View(SituacionesGuiadas.Todas);

    public IActionResult Ver(string id)
    {
        var situacion = SituacionesGuiadas.Buscar(id);
        return situacion == null ? NotFound() : View(situacion);
    }
}
