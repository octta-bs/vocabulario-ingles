using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

/// <summary>
/// Diagnóstico de nivel: un examen corto que estima tu nivel en vocabulario, gramática y escuchar, y dice qué
/// practicar y qué le falta a la app. No cuenta para tu repaso; el resultado se guarda para comparar.
/// </summary>
public class DiagnosticoController : Controller
{
    private readonly DiagnosticoService _diagnostico;

    public DiagnosticoController(DiagnosticoService diagnostico)
    {
        _diagnostico = diagnostico;
    }

    // Qué es, tus diagnósticos anteriores y el botón para empezar.
    public async Task<IActionResult> Index()
    {
        return View(await _diagnostico.HistorialAsync());
    }

    // El examen: todas las preguntas en una página; el navegador las muestra una por una.
    public async Task<IActionResult> Examen()
    {
        return View(await _diagnostico.ArmarAsync(Random.Shared));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calificar(int[] palabraId, TipoEjercicio[] tipo, string?[] respuesta)
    {
        var respuestas = palabraId
            .Select((id, i) => new RespuestaDiagnostico(id,
                i < tipo.Length ? tipo[i] : TipoEjercicio.EscribirPalabra,
                i < respuesta.Length ? respuesta[i] : null))
            .ToList();

        var id = await _diagnostico.CalificarYGuardarAsync(respuestas);
        return id == null ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Resultado), new { id });
    }

    public async Task<IActionResult> Resultado(int id)
    {
        var datos = await _diagnostico.ObtenerAsync(id);
        if (datos == null)
        {
            return NotFound();
        }

        ViewBag.Diagnostico = datos.Value.Diagnostico;
        ViewBag.Anterior = datos.Value.Anterior;
        return View(datos.Value.Resultado);
    }
}
