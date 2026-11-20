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

    private readonly RutaService _ruta;

    public HomeController(RepasoService repaso, EstadisticasService estadisticas,
        RespuestasPropiasService respuestas, ZonaHorariaUsuario zona, AppDbContext db, RutaService ruta)
    {
        _ruta = ruta;
        _repaso = repaso;
        _estadisticas = estadisticas;
        _respuestas = respuestas;
        _zona = zona;
        _db = db;
    }

    // Inicio: una sola acción principal (empezar el repaso), tu meta y accesos rápidos.
    public async Task<IActionResult> Index()
    {
        // Sin modo elegido, primero se elige (Principiante, Trabajo o Niños).
        if (Modos.Leer(Request) is not { } modo)
        {
            return RedirectToAction(nameof(Modo));
        }

        var ahora = _zona.AhoraLocal;
        var saludo = ahora.Hour switch
        {
            < 12 => "Buenos días",
            < 19 => "Buenas tardes",
            _ => "Buenas noches"
        };
        var fecha = ahora.ToString("dddd d 'de' MMMM", Espanol);

        var ritmo = await _repaso.CalcularRitmoAsync();
        var sinEmpezar = await _repaso.ContarSinEmpezarAsync(modo);

        return View(new InicioViewModel
        {
            Modo = modo,
            PendientesOtroModo = await _repaso.ContarPendientesAsync(Modos.Otro(modo)),
            Saludo = saludo,
            Fecha = char.ToUpper(fecha[0], Espanol) + fecha[1..],
            Meta = await _repaso.ObtenerMetaHoyAsync(),
            Racha = (await _estadisticas.ObtenerAsync()).RachaActual,
            Pendientes = await _repaso.ContarPendientesAsync(modo),
            NuevasPorAgregar = Math.Min(ritmo.Cantidad, sinEmpezar),
            Errores = (await _estadisticas.MasFalladasAsync(10, modo)).Count,
            Ruta = await _ruta.CalcularAsync()
        });
    }

    // Elegir modo: Principiante, Trabajo o Niños.
    public IActionResult Modo()
    {
        return View(Modos.Leer(Request));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Modo")]
    public IActionResult ElegirModo(string modo)
    {
        switch (modo)
        {
            case "ninos":
                return RedirectToAction("Index", "Ninos");
            case "principiante":
                Modos.Guardar(Response, ModoApp.Principiante);
                break;
            case "trabajo":
                Modos.Guardar(Response, ModoApp.Trabajo);
                break;
            default:
                return RedirectToAction(nameof(Modo));
        }

        return RedirectToAction(nameof(Index));
    }

    // Practicar: todas las formas de práctica extra en un solo lugar.
    public async Task<IActionResult> Practicar()
    {
        if (Modos.Leer(Request) is not { } modo)
        {
            return RedirectToAction(nameof(Modo));
        }

        var categorias = await Modos.CategoriasAsync(_db, modo);
        var errores = (await _estadisticas.MasFalladasAsync(10, modo)).Count;
        var temasEmpezados = await _db.Palabras
            .Where(p => p.Tipo == TipoPalabra.Gramatica && p.Progreso != null)
            .Select(p => p.CategoriaId)
            .Distinct()
            .CountAsync();
        return View(new PracticarViewModel
        {
            Modo = modo,
            RespuestasEscritas = (await _respuestas.ObtenerAsync()).Count,
            TotalPreguntas = PreguntasEntrevista.Todas.Length,
            Errores = errores,
            TotalPalabras = await _db.Palabras.CountAsync(p => categorias.Contains(p.CategoriaId)),
            Categorias = categorias.Count,
            TotalTemasGramatica = TemasGramatica.Todos.Length,
            TemasGramaticaEmpezados = temasEmpezados,
            Recomendacion = RecomendadorPractica.Elegir(modo, _zona.Hoy, errores, temasEmpezados,
                diagnosticoHecho: await _db.Diagnosticos.AnyAsync())
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
