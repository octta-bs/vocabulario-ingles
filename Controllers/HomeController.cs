using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class HomeController : Controller
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-MX");

    private readonly RepasoService _repaso;
    private readonly EstadisticasService _estadisticas;
    private readonly RespuestasPropiasService _respuestas;
    private readonly ZonaHorariaUsuario _zona;
    private readonly AppDbContext _db;

    public HomeController(RepasoService repaso, EstadisticasService estadisticas,
        RespuestasPropiasService respuestas, ZonaHorariaUsuario zona, AppDbContext db)
    {
        _repaso = repaso;
        _estadisticas = estadisticas;
        _respuestas = respuestas;
        _zona = zona;
        _db = db;
    }

    // Inicio: una sola acción principal (empezar el repaso), tu meta y accesos rápidos.
    public async Task<IActionResult> Index()
    {
        var ahora = _zona.AhoraLocal;
        var saludo = ahora.Hour switch
        {
            < 12 => "Buenos días",
            < 19 => "Buenas tardes",
            _ => "Buenas noches"
        };
        var fecha = ahora.ToString("dddd d 'de' MMMM", Espanol);

        var ritmo = await _repaso.CalcularRitmoAsync();
        var sinEmpezar = await _repaso.ContarSinEmpezarAsync();

        return View(new InicioViewModel
        {
            Saludo = saludo,
            Fecha = char.ToUpper(fecha[0], Espanol) + fecha[1..],
            Meta = await _repaso.ObtenerMetaHoyAsync(),
            Racha = (await _estadisticas.ObtenerAsync()).RachaActual,
            Pendientes = await _repaso.ContarPendientesAsync(),
            NuevasPorAgregar = Math.Min(ritmo.Cantidad, sinEmpezar),
            Errores = (await _estadisticas.MasFalladasAsync(10)).Count
        });
    }

    // Practicar: todas las formas de práctica extra en un solo lugar.
    public async Task<IActionResult> Practicar()
    {
        return View(new PracticarViewModel
        {
            RespuestasEscritas = (await _respuestas.ObtenerAsync()).Count,
            TotalPreguntas = PreguntasEntrevista.Todas.Length,
            Errores = (await _estadisticas.MasFalladasAsync(10)).Count,
            TotalPalabras = await _db.Palabras.CountAsync(),
            Categorias = await _db.Categorias.CountAsync()
        });
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
