namespace VocabularioIngles.Models;

public class Palabra
{
    public int Id { get; set; }

    // Cómo se escribe en inglés y qué significa.
    public string Ingles { get; set; } = "";
    public string Espanol { get; set; } = "";
    public TipoPalabra Tipo { get; set; }

    // Cómo se dice: símbolos fonéticos (IPA) y una aproximación para leerla en español.
    public string Pronunciacion { get; set; } = "";
    public string ComoSeLee { get; set; } = "";

    // Solo para verbos. El futuro no se guarda: siempre es "will" + el verbo.
    public string? Pasado { get; set; }
    public string? Participio { get; set; }

    // Solo para frases: la palabra que se quita en el ejercicio de completar ("working").
    public string? PalabraClave { get; set; }

    public string Ejemplo { get; set; } = "";
    public string EjemploEs { get; set; } = "";
    public string? Nota { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public ProgresoPalabra? Progreso { get; set; }

    public bool EsVerbo => Tipo == TipoPalabra.Verbo;
    public bool EsFrase => Tipo == TipoPalabra.Frase;
}
