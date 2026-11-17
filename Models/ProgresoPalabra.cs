namespace VocabularioIngles.Models;

/// <summary>
/// Cómo va el aprendizaje de una palabra. Solo existe cuando la palabra ya entró al repaso.
/// </summary>
public class ProgresoPalabra
{
    public int Id { get; set; }

    public int PalabraId { get; set; }
    public Palabra? Palabra { get; set; }

    // 0 = nueva. Mientras más alta la etapa, más espaciado el repaso.
    public int Etapa { get; set; }

    // A partir de esta fecha (UTC) la palabra toca repasarla.
    public DateTime ProximoRepaso { get; set; }
    public DateTime? UltimoRepaso { get; set; }

    public int Aciertos { get; set; }
    public int Fallos { get; set; }
}
