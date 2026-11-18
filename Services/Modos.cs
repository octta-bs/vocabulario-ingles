using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;

namespace VocabularioIngles.Services;

/// <summary>
/// Los modos de la app. Principiante: el vocabulario general (Pre A1 a A2). Trabajo: el inglés técnico y las frases
/// de trabajo y entrevista. Cada modo tiene su propio repaso, sus palabras nuevas y sus errores; la meta, la racha y
/// el progreso son uno solo. El modo niños tiene su propia sección y no se guarda como modo.
/// </summary>
public enum ModoApp
{
    Principiante,
    Trabajo
}

public static class Modos
{
    // La cookie donde el navegador recuerda el modo elegido.
    public const string Cookie = "modo";

    private static readonly HashSet<string> CategoriasGenerales =
        VocabularioGeneral.Categorias.Select(c => c.Nombre).ToHashSet();

    /// <summary>A qué modo pertenece una categoría.</summary>
    public static ModoApp De(string nombreCategoria) =>
        CategoriasGenerales.Contains(nombreCategoria) ? ModoApp.Principiante : ModoApp.Trabajo;

    public static ModoApp Otro(ModoApp modo) => modo == ModoApp.Principiante ? ModoApp.Trabajo : ModoApp.Principiante;

    public static string Texto(this ModoApp modo) => modo == ModoApp.Principiante ? "Principiante" : "Trabajo";

    /// <summary>Los ids de las categorías de un modo.</summary>
    public static async Task<List<int>> CategoriasAsync(AppDbContext db, ModoApp modo)
    {
        var categorias = await db.Categorias.Select(c => new { c.Id, c.Nombre }).ToListAsync();
        return categorias.Where(c => De(c.Nombre) == modo).Select(c => c.Id).ToList();
    }

    public static ModoApp? Leer(HttpRequest request) => request.Cookies[Cookie] switch
    {
        "principiante" => ModoApp.Principiante,
        "trabajo" => ModoApp.Trabajo,
        _ => null
    };

    public static void Guardar(HttpResponse response, ModoApp modo)
    {
        response.Cookies.Append(Cookie, modo == ModoApp.Principiante ? "principiante" : "trabajo",
            new CookieOptions { MaxAge = TimeSpan.FromDays(365), HttpOnly = true, IsEssential = true, SameSite = SameSiteMode.Lax });
    }
}
