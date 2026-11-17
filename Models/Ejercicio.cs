namespace VocabularioIngles.Models;

/// <summary>Una pregunta concreta del repaso, armada a partir de una palabra.</summary>
public class Ejercicio
{
    public Palabra Palabra { get; set; } = null!;
    public TipoEjercicio Tipo { get; set; }

    // Lo que hay que escribir para acertar.
    public string Respuesta { get; set; } = "";

    // Solo en los ejercicios de completar: la frase con el hueco ("Yesterday I ___ a bug."),
    // la frase entera y su traducción.
    public string? FraseConHueco { get; set; }
    public string? FraseCompleta { get; set; }
    public string? FraseEs { get; set; }

    // "presente", "pasado" o "futuro" cuando la palabra es un verbo.
    public string? TiempoVerbal { get; set; }

    public bool EsCompletar => Tipo != TipoEjercicio.EscribirPalabra;
}
