namespace VocabularioIngles.Models;

/// <summary>
/// Niveles del Marco Común Europeo de Referencia (MCER), el mismo orden que usa Cambridge:
/// de Pre A1 (preescolar) a C2 (dominio).
/// </summary>
public enum NivelMcer
{
    PreA1,
    A1,
    A2,
    B1,
    B2,
    C1,
    C2
}

public static class NivelMcerExtensiones
{
    /// <summary>Cómo se escribe el nivel para mostrarlo ("Pre A1", "B2"…).</summary>
    public static string Texto(this NivelMcer nivel) => nivel == NivelMcer.PreA1 ? "Pre A1" : nivel.ToString();

    /// <summary>Una descripción corta del nivel.</summary>
    public static string Descripcion(this NivelMcer nivel) => nivel switch
    {
        NivelMcer.PreA1 => "Desde cero",
        NivelMcer.A1 => "Principiante",
        NivelMcer.A2 => "Básico",
        NivelMcer.B1 => "Intermedio",
        NivelMcer.B2 => "Intermedio alto",
        NivelMcer.C1 => "Avanzado",
        _ => "Dominio"
    };
}
