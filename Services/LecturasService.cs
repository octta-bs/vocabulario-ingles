using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>
/// Leer en contexto: guardas textos reales en inglés, tocas las palabras que no conoces y entran a tu repaso
/// con la frase donde aparecieron como ejemplo.
/// </summary>
public class LecturasService
{
    public const string NombreCategoria = "Mis lecturas";
    public const int LargoMaximoFrase = 400;
    public const int LargoMaximoSignificado = 120;

    // Una palabra: letras, con apóstrofo o guion dentro (don't, e-mail).
    private static readonly Regex Palabras = new(@"[A-Za-z]+(?:['’\-][A-Za-z]+)*", RegexOptions.Compiled);

    private readonly AppDbContext _db;
    private readonly ZonaHorariaUsuario _zona;

    public LecturasService(AppDbContext db, ZonaHorariaUsuario zona)
    {
        _db = db;
        _zona = zona;
    }

    public Task<List<Lectura>> ListarAsync()
    {
        return _db.Lecturas.OrderByDescending(l => l.Creada).ToListAsync();
    }

    /// <summary>Guarda un texto nuevo. Devuelve null si el texto viene vacío.</summary>
    public async Task<Lectura?> CrearAsync(string? titulo, string? texto)
    {
        var limpio = (texto ?? "").Replace("\r\n", "\n").Trim();
        if (limpio.Length == 0)
        {
            return null;
        }

        if (limpio.Length > Lectura.LargoMaximoTexto)
        {
            limpio = limpio[..Lectura.LargoMaximoTexto];
        }

        var nombre = (titulo ?? "").Trim();
        if (nombre.Length == 0)
        {
            // Sin título: las primeras palabras del texto.
            nombre = string.Join(' ', limpio.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Take(6));
        }

        if (nombre.Length > Lectura.LargoMaximoTitulo)
        {
            nombre = nombre[..Lectura.LargoMaximoTitulo];
        }

        var lectura = new Lectura { Titulo = nombre, Texto = limpio, Creada = _zona.AhoraUtc };
        _db.Lecturas.Add(lectura);
        await _db.SaveChangesAsync();
        return lectura;
    }

    public async Task<bool> BorrarAsync(int id)
    {
        var lectura = await _db.Lecturas.FindAsync(id);
        if (lectura == null)
        {
            return false;
        }

        _db.Lecturas.Remove(lectura);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// La lectura partida en segmentos (palabras que se pueden tocar y lo que hay entre ellas) y en oraciones.
    /// Las palabras que ya estás aprendiendo vienen marcadas.
    /// </summary>
    public async Task<LecturaParaLeer?> AbrirAsync(int id)
    {
        var lectura = await _db.Lecturas.FindAsync(id);
        if (lectura == null)
        {
            return null;
        }

        var aprendiendo = (await _db.Progresos.Select(p => p.Palabra!.Ingles).ToListAsync())
            .Select(i => i.ToLowerInvariant())
            .ToHashSet();

        var (segmentos, oraciones) = Partir(lectura.Texto, aprendiendo);
        return new LecturaParaLeer(lectura, segmentos, oraciones);
    }

    /// <summary>Parte un texto en segmentos y oraciones (una oración termina en . ! ? o en un salto de línea).</summary>
    public static (List<SegmentoLectura> Segmentos, List<string> Oraciones) Partir(string texto, ISet<string> aprendiendo)
    {
        var oraciones = Regex.Split(texto, @"(?<=[.!?])\s+|\n+")
            .Select(o => o.Trim())
            .Where(o => o.Length > 0)
            .ToList();

        var segmentos = new List<SegmentoLectura>();
        var oracion = 0;
        var posicionEnOracion = 0;
        var ultimo = 0;
        foreach (Match m in Palabras.Matches(texto))
        {
            if (m.Index > ultimo)
            {
                segmentos.Add(new SegmentoLectura(texto[ultimo..m.Index], false, oracion, false));
            }

            // Avanza a la oración que contiene esta palabra.
            while (oracion < oraciones.Count - 1 && oraciones[oracion].IndexOf(m.Value, posicionEnOracion, StringComparison.Ordinal) < 0)
            {
                oracion++;
                posicionEnOracion = 0;
            }

            if (oracion < oraciones.Count)
            {
                var donde = oraciones[oracion].IndexOf(m.Value, posicionEnOracion, StringComparison.Ordinal);
                posicionEnOracion = donde < 0 ? posicionEnOracion : donde + m.Length;
            }

            segmentos.Add(new SegmentoLectura(m.Value, true, oracion, aprendiendo.Contains(m.Value.ToLowerInvariant())));
            ultimo = m.Index + m.Length;
        }

        if (ultimo < texto.Length)
        {
            segmentos.Add(new SegmentoLectura(texto[ultimo..], false, oracion, false));
        }

        return (segmentos, oraciones);
    }

    /// <summary>El significado de una palabra si ya existe en la app (para no escribirlo de nuevo).</summary>
    public async Task<string?> BuscarSignificadoAsync(string? palabra)
    {
        var clave = (palabra ?? "").Trim().ToLowerInvariant();
        if (clave.Length == 0)
        {
            return null;
        }

        return await _db.Palabras
            .Where(p => p.Ingles.ToLower() == clave)
            .Select(p => p.Espanol)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Mete una palabra de la lectura a tu repaso. Si ya existe en la app, se usa esa (no se duplica); si no,
    /// se crea en la categoría "Mis lecturas" con la frase como ejemplo y el significado que escribiste.
    /// </summary>
    public async Task<ResultadoGuardarPalabra> GuardarPalabraAsync(string? palabra, string? frase, string? significado)
    {
        var ingles = (palabra ?? "").Trim().ToLowerInvariant().Replace('’', '\'');
        if (!Palabras.IsMatch(ingles) || Palabras.Match(ingles).Value != ingles || ingles.Length > 60)
        {
            return new ResultadoGuardarPalabra(false, "Esa no parece una palabra en inglés.");
        }

        var existente = await _db.Palabras.Include(p => p.Progreso)
            .FirstOrDefaultAsync(p => p.Ingles.ToLower() == ingles);
        var ahora = _zona.AhoraUtc;

        if (existente != null)
        {
            if (existente.Progreso != null)
            {
                return new ResultadoGuardarPalabra(true, $"\"{existente.Ingles}\" ya está en tu repaso.", existente.Espanol);
            }

            _db.Progresos.Add(new ProgresoPalabra { PalabraId = existente.Id, ProximoRepaso = ahora });
            await _db.SaveChangesAsync();
            return new ResultadoGuardarPalabra(true, $"\"{existente.Ingles}\" ya existía en la app: la agregué a tu repaso.", existente.Espanol);
        }

        var espanol = (significado ?? "").Trim();
        if (espanol.Length == 0)
        {
            return new ResultadoGuardarPalabra(false, "Escribe qué significa para poder guardarla.");
        }

        if (espanol.Length > LargoMaximoSignificado)
        {
            espanol = espanol[..LargoMaximoSignificado];
        }

        var ejemplo = (frase ?? "").Trim();
        if (ejemplo.Length > LargoMaximoFrase || !Regex.IsMatch(ejemplo, @"\b" + Regex.Escape(ingles) + @"\b", RegexOptions.IgnoreCase))
        {
            // Sin una frase que contenga la palabra no hay ejercicio de completar; se guarda sin ejemplo.
            ejemplo = "";
        }

        var nueva = new Palabra
        {
            Categoria = await CategoriaAsync(),
            Tipo = TipoPalabra.Sustantivo,
            Ingles = ingles,
            Espanol = espanol,
            Ejemplo = ejemplo
        };
        _db.Palabras.Add(nueva);
        _db.Progresos.Add(new ProgresoPalabra { Palabra = nueva, ProximoRepaso = ahora });
        await _db.SaveChangesAsync();
        return new ResultadoGuardarPalabra(true, $"Guardé \"{ingles}\" en tu repaso.", espanol);
    }

    /// <summary>La categoría "Mis lecturas" (modo Trabajo, nivel B1); se crea la primera vez.</summary>
    private async Task<Categoria> CategoriaAsync()
    {
        var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Nombre == NombreCategoria);
        if (categoria == null)
        {
            categoria = new Categoria { Nombre = NombreCategoria, Nivel = NivelMcer.B1 };
            _db.Categorias.Add(categoria);
        }

        return categoria;
    }
}

/// <summary>Cómo salió guardar una palabra de una lectura.</summary>
public record ResultadoGuardarPalabra(bool Guardada, string Mensaje, string? Significado = null);
