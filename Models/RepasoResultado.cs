namespace VocabularioIngles.Models;

public class RepasoResultado
{
    public bool Acerto { get; set; }
    public string RespuestaDada { get; set; } = "";
    public Palabra Palabra { get; set; } = null!;
}
