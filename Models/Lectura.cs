namespace VocabularioIngles.Models;

/// <summary>Un texto en inglés que pegaste para leer (documentación, un README, un correo…).</summary>
public class Lectura
{
    public const int LargoMaximoTitulo = 120;
    public const int LargoMaximoTexto = 20000;

    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Texto { get; set; } = "";

    // Cuándo la guardaste (UTC).
    public DateTime Creada { get; set; }
}

/// <summary>
/// Un pedazo del texto al mostrarlo: una palabra que se puede tocar o lo que hay entre palabras (espacios,
/// puntuación, saltos de línea). <see cref="Oracion"/> dice en qué oración está, para guardarla como ejemplo.
/// </summary>
public record SegmentoLectura(string Texto, bool EsPalabra, int Oracion, bool YaLaAprendes);

/// <summary>Una lectura lista para mostrarse: sus segmentos y sus oraciones.</summary>
public record LecturaParaLeer(Lectura Lectura, List<SegmentoLectura> Segmentos, List<string> Oraciones);
