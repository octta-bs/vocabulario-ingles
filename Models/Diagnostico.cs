namespace VocabularioIngles.Models;

/// <summary>Las áreas que mide el diagnóstico de nivel.</summary>
public enum AreaDiagnostico
{
    Vocabulario,
    Gramatica,
    Escucha
}

/// <summary>
/// Un diagnóstico de nivel ya calificado. Se guarda para comparar tu avance de un mes a otro.
/// Los niveles son null cuando todavía no se alcanza el primer nivel que mide esa área.
/// </summary>
public class Diagnostico
{
    public int Id { get; set; }

    // Cuándo se hizo (UTC).
    public DateTime Fecha { get; set; }

    public int Aciertos { get; set; }
    public int Total { get; set; }

    public NivelMcer? NivelVocabulario { get; set; }
    public NivelMcer? NivelGramatica { get; set; }
    public NivelMcer? NivelEscucha { get; set; }

    // El resultado completo (áreas, puntos débiles y lo que le falta a la app) en JSON, para volver a mostrarlo.
    public string Detalle { get; set; } = "";
}

/// <summary>Una pregunta del diagnóstico: el ejercicio y qué mide.</summary>
public record PreguntaDiagnostico(AreaDiagnostico Area, NivelMcer Nivel, Ejercicio Ejercicio);

/// <summary>Cómo te fue en un área: tu nivel estimado y hasta qué nivel llega el contenido de la app.</summary>
public record ResultadoArea(AreaDiagnostico Area, NivelMcer? Nivel, int Aciertos, int Total, NivelMcer NivelMaximo);

/// <summary>Algo para practicar, con el enlace a donde se practica.</summary>
public record SugerenciaDiagnostico(string Texto, string Controlador, string Accion, string? Id = null);

/// <summary>El resultado completo de un diagnóstico.</summary>
public record ResultadoDiagnostico(
    List<ResultadoArea> Areas,
    List<SugerenciaDiagnostico> PuntosDebiles,
    List<string> LeFalta,
    List<string> TemasFallados)
{
    public int Aciertos => Areas.Sum(a => a.Aciertos);
    public int Total => Areas.Sum(a => a.Total);

    public ResultadoArea Area(AreaDiagnostico area) => Areas.Single(a => a.Area == area);
}

public static class AreaDiagnosticoExtensiones
{
    public static string Texto(this AreaDiagnostico area) => area switch
    {
        AreaDiagnostico.Vocabulario => "Vocabulario",
        AreaDiagnostico.Gramatica => "Gramática",
        _ => "Escuchar"
    };
}
