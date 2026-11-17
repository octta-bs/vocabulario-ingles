namespace VocabularioIngles.Services;

/// <summary>Lo que decide el algoritmo después de cada respuesta.</summary>
public record ResultadoAlgoritmo(int NuevaEtapa, DateTime ProximoRepaso);

/// <summary>
/// Patrón Strategy: cualquier forma de calcular cuándo volver a ver una palabra
/// (intervalos fijos hoy, FSRS mañana) cumple este contrato.
/// </summary>
public interface IAlgoritmoRepaso
{
    ResultadoAlgoritmo Calcular(int etapaActual, bool acerto, DateTime ahora);
}
