namespace VocabularioIngles.Models;

/// <summary>
/// Tu propia respuesta a una pregunta del simulacro de entrevista. Se guarda en tu base de datos
/// (no en el repositorio), así que puedes poner tus datos reales.
/// </summary>
public class RespuestaPropia
{
    public const int LargoMaximo = 2000;

    public int Id { get; set; }

    // La clave fija de la pregunta (PreguntasEntrevista.Pregunta.Clave), por ejemplo "tell-me-about-yourself".
    public string ClavePregunta { get; set; } = "";

    public string Texto { get; set; } = "";

    // Cuándo la guardaste por última vez (UTC).
    public DateTime Actualizada { get; set; }
}
