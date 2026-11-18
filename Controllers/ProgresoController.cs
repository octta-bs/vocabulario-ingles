using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

namespace VocabularioIngles.Controllers;

public class ProgresoController : Controller
{
    private readonly EstadisticasService _estadisticas;
    private readonly AppDbContext _db;

    public ProgresoController(EstadisticasService estadisticas, AppDbContext db)
    {
        _estadisticas = estadisticas;
        _db = db;
    }

    // Racha, ejercicios por día y palabras que más fallas.
    public async Task<IActionResult> Index()
    {
        return View(await _estadisticas.ObtenerAsync());
    }

    // Descarga una copia completa de tu base (tu progreso) para guardarla donde quieras.
    public async Task<IActionResult> Respaldo()
    {
        var temporal = Path.Combine(Path.GetTempPath(), $"vocabulario-{Guid.NewGuid():N}.db");
        try
        {
            // VACUUM INTO copia la base de forma segura aunque la app la esté usando.
            await _db.Database.ExecuteSqlAsync($"VACUUM INTO {temporal}");
            var contenido = await System.IO.File.ReadAllBytesAsync(temporal);
            return File(contenido, "application/vnd.sqlite3", $"vocabulario-respaldo-{DateTime.Now:yyyyMMdd-HHmm}.db");
        }
        finally
        {
            System.IO.File.Delete(temporal);
        }
    }
}
