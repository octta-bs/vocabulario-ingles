namespace VocabularioIngles.Models;

public class ResumenViewModel
{
    public int TotalPalabras { get; set; }
    public int EnRepaso { get; set; }
    public int Pendientes { get; set; }
    public int Dominadas { get; set; }
    public MetaDelDia Meta { get; set; } = new(Configuracion.MetaPorDefecto, 0);
}
