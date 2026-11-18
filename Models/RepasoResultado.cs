namespace VocabularioIngles.Models;

public class RepasoResultado
{
    public bool Acerto { get; set; }
    public string RespuestaDada { get; set; } = "";
    public Palabra Palabra { get; set; } = null!;

    // El ejercicio que se respondió (escribir la palabra o completar una frase).
    public Ejercicio Ejercicio { get; set; } = null!;
}
