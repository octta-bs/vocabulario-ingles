using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>Guarda y lee tus propias respuestas a las preguntas del simulacro de entrevista.</summary>
public class RespuestasPropiasService
{
    private readonly AppDbContext _db;
    private readonly ZonaHorariaUsuario _zona;

    public RespuestasPropiasService(AppDbContext db, ZonaHorariaUsuario zona)
    {
        _db = db;
        _zona = zona;
    }

    /// <summary>Tus respuestas por clave de pregunta (solo las que escribiste).</summary>
    public Task<Dictionary<string, string>> ObtenerAsync()
    {
        return _db.RespuestasPropias.ToDictionaryAsync(r => r.ClavePregunta, r => r.Texto);
    }

    /// <summary>
    /// Guarda las respuestas recibidas. Una respuesta vacía se borra. Las claves que no son de
    /// ninguna pregunta se ignoran, y el texto se recorta al largo máximo.
    /// </summary>
    public async Task GuardarAsync(IDictionary<string, string?> respuestas)
    {
        var validas = PreguntasEntrevista.Todas.Select(p => p.Clave).ToHashSet();
        var guardadas = await _db.RespuestasPropias.ToDictionaryAsync(r => r.ClavePregunta);

        foreach (var (clave, textoRecibido) in respuestas)
        {
            if (!validas.Contains(clave))
            {
                continue;
            }

            var texto = (textoRecibido ?? "").Trim();
            if (texto.Length > RespuestaPropia.LargoMaximo)
            {
                texto = texto[..RespuestaPropia.LargoMaximo];
            }

            guardadas.TryGetValue(clave, out var existente);
            if (texto.Length == 0)
            {
                if (existente != null)
                {
                    _db.RespuestasPropias.Remove(existente);
                }
            }
            else if (existente == null)
            {
                _db.RespuestasPropias.Add(new RespuestaPropia { ClavePregunta = clave, Texto = texto, Actualizada = _zona.AhoraUtc });
            }
            else if (existente.Texto != texto)
            {
                existente.Texto = texto;
                existente.Actualizada = _zona.AhoraUtc;
            }
        }

        await _db.SaveChangesAsync();
    }
}
