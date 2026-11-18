namespace VocabularioIngles.Services;

/// <summary>
/// Cuántas palabras nuevas te tocan y por qué. <see cref="PorcentajeAciertos"/> es null cuando
/// todavía no hay suficientes respuestas para calcularlo.
/// </summary>
public record RitmoNuevas(int Cantidad, int? PorcentajeAciertos);

/// <summary>
/// Patrón Strategy: cualquier regla para decidir cuántas palabras nuevas meter al repaso
/// según cómo te va cumple este contrato.
/// </summary>
public interface IRitmoPalabrasNuevas
{
    /// <summary>Cuántas de tus últimas respuestas quiere ver esta regla.</summary>
    int RespuestasQueUsa { get; }

    /// <param name="ultimasRespuestas">Tus últimas respuestas (true = acierto), de la más reciente a la más vieja.</param>
    RitmoNuevas Calcular(IReadOnlyList<bool> ultimasRespuestas);
}
