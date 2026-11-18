using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Simulacro de entrevista: 5 preguntas en inglés que respondes en voz alta o por escrito,
/// y después ves tu propia respuesta (si la escribiste en "Mis respuestas") o un ejemplo.
/// Es solo práctica: no se guarda nada.
/// </summary>
public class SimulacroController : Controller
{
    private readonly RespuestasPropiasService _respuestas;

    public SimulacroController(RespuestasPropiasService respuestas)
    {
        _respuestas = respuestas;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.MisRespuestas = await _respuestas.ObtenerAsync();
        return View(PreguntasEntrevista.ElegirSimulacro(Random.Shared));
    }
}
