namespace VocabularioIngles.Models;

/// <summary>
/// Una respuesta del repaso. Con este historial la app calcula tus fallos recientes por palabra,
/// tu porcentaje de aciertos (para el ritmo de palabras nuevas) y cuánto llevas de tu meta de hoy.
/// </summary>
public class RegistroRespuesta
{
    public int Id { get; set; }

    public int PalabraId { get; set; }
    public Palabra? Palabra { get; set; }

    public TipoEjercicio Tipo { get; set; }
    public bool Acerto { get; set; }

    // Cuándo respondiste (UTC).
    public DateTime Fecha { get; set; }
}
