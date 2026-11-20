using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class RepasoController : Controller
{
    private readonly RepasoService _servicio;
    private readonly EstadisticasService _estadisticas;
    private readonly GeneradorEjercicios _generador;

    public RepasoController(RepasoService servicio, EstadisticasService estadisticas, GeneradorEjercicios generador)
    {
        _servicio = servicio;
        _estadisticas = estadisticas;
        _generador = generador;
    }

    // Cookie donde se recuerda la última categoría elegida para las palabras nuevas.
    public const string CookieCategoria = "categoriaNuevas";

    // Muestra el siguiente ejercicio pendiente del modo actual (o avisa que no hay).
    public async Task<IActionResult> Index()
    {
        if (Modos.Leer(Request) is not { } modo)
        {
            return RedirectToAction("Modo", "Home");
        }

        var ejercicio = await _servicio.SiguienteEjercicioAsync(modo);
        ViewBag.Modo = modo;
        ViewBag.Pendientes = await _servicio.ContarPendientesAsync(modo);
        ViewBag.PendientesOtroModo = await _servicio.ContarPendientesAsync(Modos.Otro(modo));
        ViewBag.Meta = await _servicio.ObtenerMetaHoyAsync();
        return View(ejercicio);
    }

    // Cuántas palabras nuevas te tocan y de qué categoría agregarlas.
    public async Task<IActionResult> Nuevas()
    {
        if (Modos.Leer(Request) is not { } modo)
        {
            return RedirectToAction("Modo", "Home");
        }

        ViewBag.Modo = modo;
        ViewBag.Ritmo = await _servicio.CalcularRitmoAsync();
        ViewBag.SinEmpezar = await _servicio.ContarSinEmpezarAsync(modo);
        ViewBag.Categorias = await _servicio.CategoriasConNuevasAsync(modo);
        ViewBag.CategoriaElegida = int.TryParse(Request.Cookies[CookieCategoria], out var elegida) ? elegida : (int?)null;
        return View();
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

    // Práctica extra con las palabras que más has fallado (no cambia el repaso ni la meta).
    public async Task<IActionResult> Errores(int? anterior)
    {
        var dificiles = await _estadisticas.MasFalladasAsync(10, Modos.Leer(Request));
        if (dificiles.Count > 1 && anterior.HasValue)
        {
            dificiles.RemoveAll(d => d.Palabra.Id == anterior.Value);
        }

        if (dificiles.Count == 0)
        {
            return View((Ejercicio?)null);
        }

        var palabra = dificiles[Random.Shared.Next(dificiles.Count)].Palabra;
        return View(_generador.Elegir(palabra));
    }

    [HttpPost]
    public async Task<IActionResult> ResponderError(int palabraId, TipoEjercicio tipo, string? respuesta)
    {
        var resultado = await _servicio.EvaluarSinGuardarAsync(palabraId, tipo, respuesta);
        if (resultado == null)
        {
            return RedirectToAction(nameof(Errores));
        }

        ViewBag.EsPractica = true;
        ViewBag.SiguienteAccion = nameof(Errores);
        ViewBag.Anterior = palabraId;
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
        var cantidad = await _servicio.AgregarNuevasAsync(categoriaId, Modos.Leer(Request));
        if (cantidad > 0)
        {
            // Las nuevas entran como pendientes: el repaso empieza con ellas.
            TempData["Mensaje"] = $"Se agregaron {cantidad} palabras nuevas a tu repaso.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Mensaje"] = ritmo.Cantidad == 0
            ? "Por ahora no te tocan palabras nuevas: primero refuerza las que ya tienes."
            : "Ya agregaste todas las palabras disponibles.";
        return RedirectToAction(nameof(Nuevas));
    }
}
