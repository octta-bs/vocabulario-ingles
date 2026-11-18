namespace VocabularioIngles.Services;

/// <summary>Lo que decide el algoritmo después de cada respuesta.</summary>
public record ResultadoAlgoritmo(int NuevaEtapa, DateTime ProximoRepaso);

/// <summary>
/// Patrón Strategy: cualquier forma de calcular cuándo volver a ver una palabra
/// (intervalos fijos, intervalos adaptativos, FSRS mañana) cumple este contrato.
/// </summary>
public interface IAlgoritmoRepaso
{
    /// <param name="etapaActual">La etapa de la palabra antes de responder.</param>
    /// <param name="acerto">Si la respuesta de ahora fue correcta.</param>
    /// <param name="ahora">La hora actual (UTC).</param>
    /// <param name="ultimasRespuestas">
    /// Las últimas respuestas de esta palabra (true = acierto), de la más reciente a la más vieja,
    /// incluyendo la de ahora. Cada estrategia decide si la usa.
    /// </param>
    ResultadoAlgoritmo Calcular(int etapaActual, bool acerto, DateTime ahora, IReadOnlyList<bool> ultimasRespuestas);
}
