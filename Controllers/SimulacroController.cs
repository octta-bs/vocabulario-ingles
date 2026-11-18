using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Data;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Simulacro de entrevista: 5 preguntas en inglés que respondes en voz alta o por escrito,
/// y después ves una respuesta modelo. Es solo práctica: no se guarda nada.
/// </summary>
public class SimulacroController : Controller
{
    public IActionResult Index()
    {
        return View(PreguntasEntrevista.ElegirSimulacro(Random.Shared));
    }
}
