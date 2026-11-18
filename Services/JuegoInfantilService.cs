using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>
/// El modo infantil: categorías con dibujo, el juego "Escucha y toca" y el progreso del niño
/// (separado del progreso del adulto).
/// </summary>
public class JuegoInfantilService
{
    public const int RondasPorJuego = 8;
    public const int OpcionesPorRonda = 3;

    private readonly AppDbContext _db;
    private readonly ZonaHorariaUsuario _zona;

    public JuegoInfantilService(AppDbContext db, ZonaHorariaUsuario zona)
    {
        _db = db;
        _zona = zona;
    }

    /// <summary>Las categorías del modo infantil que existen en la base, en el orden en que se juegan.</summary>
    public async Task<List<CategoriaInfantil>> CategoriasAsync()
    {
        var categorias = await _db.Categorias
            .Where(c => DibujosInfantiles.Categorias.Contains(c.Nombre))
            .Select(c => new
            {
                c.Id,
                c.Nombre,
                Palabras = c.Palabras.Select(p => p.Ingles).ToList(),
                Aprendidas = _db.ProgresosInfantiles.Count(p => p.Palabra!.CategoriaId == c.Id
                    && p.AciertosSeguidos >= ProgresoInfantil.AciertosParaAprender)
            })
            .ToListAsync();

        return categorias
            .OrderBy(c => Array.IndexOf(DibujosInfantiles.Categorias, c.Nombre))
            .Select(c =>
            {
                var conDibujo = c.Palabras.Where(DibujosInfantiles.Existe).ToList();
                return new CategoriaInfantil(c.Id, c.Nombre, conDibujo.Count, c.Aprendidas, conDibujo.FirstOrDefault() ?? "");
            })
            .ToList();
    }

    public async Task<CategoriaInfantil?> CategoriaAsync(int categoriaId)
    {
        return (await CategoriasAsync()).FirstOrDefault(c => c.Id == categoriaId);
    }

    /// <summary>Las palabras con dibujo de una categoría infantil, en orden.</summary>
    public async Task<List<PalabraInfantil>> PalabrasAsync(int categoriaId)
    {
        var palabras = await PalabrasConProgresoAsync(categoriaId);
        return palabras.Select(p => p.Palabra).ToList();
    }

    /// <summary>
    /// Arma un juego de 8 rondas. Primero salen las palabras que el niño acaba de fallar, luego las que
    /// aún no aprende y al final las aprendidas. Cada ronda tiene la palabra y 2 opciones más de la misma
    /// categoría, revueltas.
    /// </summary>
    public async Task<List<RondaInfantil>> ArmarJuegoAsync(int categoriaId, Random azar)
    {
        var palabras = await PalabrasConProgresoAsync(categoriaId);
        if (palabras.Count < OpcionesPorRonda)
        {
            return new List<RondaInfantil>();
        }

        var ordenadas = palabras
            .Select(p => (p.Palabra, Prioridad: Prioridad(p.Progreso), Azar: azar.Next()))
            .OrderBy(p => p.Prioridad)
            .ThenBy(p => p.Azar)
            .Select(p => p.Palabra)
            .ToList();

        var rondas = new List<RondaInfantil>();
        for (var i = 0; i < RondasPorJuego; i++)
        {
            var objetivo = ordenadas[i % ordenadas.Count];
            var opciones = palabras.Select(p => p.Palabra)
                .Where(p => p.Id != objetivo.Id)
                .OrderBy(_ => azar.Next())
                .Take(OpcionesPorRonda - 1)
                .Append(objetivo)
                .OrderBy(_ => azar.Next())
                .ToList();
            rondas.Add(new RondaInfantil(objetivo.Id, objetivo.Ingles, opciones));
        }

        return rondas;
    }

    /// <summary>0 = la falló la última vez, 1 = todavía no la aprende (o nunca la jugó), 2 = ya aprendida.</summary>
    private static int Prioridad(ProgresoInfantil? progreso)
    {
        if (progreso == null)
        {
            return 1;
        }

        if (progreso.Aprendida)
        {
            return 2;
        }

        return progreso.Fallos > 0 && progreso.AciertosSeguidos == 0 ? 0 : 1;
    }

    /// <summary>Guarda una respuesta del juego. Devuelve false si la palabra no es del modo infantil.</summary>
    public async Task<bool> RegistrarAsync(int palabraId, bool acerto)
    {
        var palabra = await _db.Palabras.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == palabraId);
        if (palabra?.Categoria == null || !DibujosInfantiles.Categorias.Contains(palabra.Categoria.Nombre)
            || !DibujosInfantiles.Existe(palabra.Ingles))
        {
            return false;
        }

        var progreso = await _db.ProgresosInfantiles.FirstOrDefaultAsync(p => p.PalabraId == palabraId);
        if (progreso == null)
        {
            progreso = new ProgresoInfantil { PalabraId = palabraId };
            _db.ProgresosInfantiles.Add(progreso);
        }

        if (acerto)
        {
            progreso.Aciertos++;
            progreso.AciertosSeguidos++;
        }
        else
        {
            progreso.Fallos++;
            progreso.AciertosSeguidos = 0;
        }

        progreso.UltimaVez = _zona.AhoraUtc;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string> ObtenerNombreAsync()
    {
        return await _db.PerfilesInfantiles.OrderBy(p => p.Id).Select(p => p.Nombre).FirstOrDefaultAsync() ?? "";
    }

    public async Task GuardarNombreAsync(string? nombre)
    {
        var limpio = (nombre ?? "").Trim();
        if (limpio.Length > PerfilInfantil.LargoMaximoNombre)
        {
            limpio = limpio[..PerfilInfantil.LargoMaximoNombre];
        }

        var perfil = await _db.PerfilesInfantiles.OrderBy(p => p.Id).FirstOrDefaultAsync();
        if (perfil == null)
        {
            _db.PerfilesInfantiles.Add(new PerfilInfantil { Nombre = limpio });
        }
        else
        {
            perfil.Nombre = limpio;
        }

        await _db.SaveChangesAsync();
    }

    private async Task<List<(PalabraInfantil Palabra, ProgresoInfantil? Progreso)>> PalabrasConProgresoAsync(int categoriaId)
    {
        var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == categoriaId);
        if (categoria == null || !DibujosInfantiles.Categorias.Contains(categoria.Nombre))
        {
            return new();
        }

        var palabras = await _db.Palabras.Where(p => p.CategoriaId == categoriaId).OrderBy(p => p.Id).ToListAsync();
        var ids = palabras.Select(p => p.Id).ToList();
        var progresos = await _db.ProgresosInfantiles.Where(p => ids.Contains(p.PalabraId)).ToDictionaryAsync(p => p.PalabraId);

        return palabras
            .Where(p => DibujosInfantiles.Existe(p.Ingles))
            .Select(p =>
            {
                progresos.TryGetValue(p.Id, out var progreso);
                return (new PalabraInfantil(p.Id, p.Ingles, p.Espanol, progreso?.Aprendida == true), progreso);
            })
            .ToList();
    }
}
