namespace VocabularioIngles.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public List<Palabra> Palabras { get; set; } = new();
}
