namespace VocabularioIngles.Models;

/// <summary>Preferencias del usuario. Solo hay una fila (la app es de una sola persona).</summary>
public class Configuracion
{
    public const int MetaMinima = 5;
    public const int MetaMaxima = 200;
    public const int MetaPorDefecto = 30;

    public int Id { get; set; }

    // Cuántos ejercicios quieres responder al día (aciertes o falles).
    public int MetaDiaria { get; set; } = MetaPorDefecto;
}
