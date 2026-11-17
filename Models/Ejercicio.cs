namespace VocabularioIngles.Models;

/// <summary>Una pregunta concreta del repaso, armada a partir de una palabra.</summary>
public class Ejercicio
{
    public Palabra Palabra { get; set; } = null!;
    public TipoEjercicio Tipo { get; set; }

    // Lo que hay que escribir para acertar.
    public string Respuesta { get; set; } = "";

    // En los ejercicios de completar (y en el dictado de frase): la frase con el hueco ("Yesterday I ___ a bug."),
    // la frase entera y su traducción.
    public string? FraseConHueco { get; set; }
    public string? FraseCompleta { get; set; }
    public string? FraseEs { get; set; }

    // "presente", "pasado" o "futuro" cuando la palabra es un verbo.
    public string? TiempoVerbal { get; set; }

    // Solo en el dictado: lo que lee en voz alta el navegador.
    public string? TextoAudio { get; set; }

    public bool EsCompletar => Tipo is TipoEjercicio.CompletarFrase
        or TipoEjercicio.CompletarPasado or TipoEjercicio.CompletarFuturo;

    public bool EsDictado => Tipo is TipoEjercicio.DictadoPalabra or TipoEjercicio.DictadoFrase;
}
