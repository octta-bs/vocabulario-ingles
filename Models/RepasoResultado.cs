namespace VocabularioIngles.Models;

public class RepasoResultado
{
    public bool Acerto { get; set; }
    public string RespuestaDada { get; set; } = "";
    public Palabra Palabra { get; set; } = null!;

    // El ejercicio que se respondió (escribir la palabra o completar una frase).
    public Ejercicio Ejercicio { get; set; } = null!;

    // Si falló: por qué (tiempo verbal, una letra, otra palabra…). Vacío si no hay una explicación clara.
    public List<string> Explicaciones { get; set; } = new();
}
