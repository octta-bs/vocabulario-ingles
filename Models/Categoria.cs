namespace VocabularioIngles.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";

    // Nivel del MCER: las palabras nuevas se agregan de los niveles bajos a los altos.
    public NivelMcer Nivel { get; set; } = NivelMcer.A2;
    public List<Palabra> Palabras { get; set; } = new();
}
