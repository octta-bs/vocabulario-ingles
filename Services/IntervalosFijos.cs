namespace VocabularioIngles.Services;

/// <summary>
/// Algoritmo sencillo: cada acierto sube una etapa y la palabra tarda más en volver.
/// Un error la regresa a la etapa 0. No usa el historial de respuestas.
/// </summary>
public class IntervalosFijos : IAlgoritmoRepaso
{
    // Días de espera según la etapa a la que se llega. Un acierto ya no la repite el mismo día:
    // pasa a mañana y después se va espaciando: 1, 3, 7, 14 y 30 días. Un fallo la deja para el final
    // de la sesión de hoy (vuelve después de las demás pendientes).
    private static readonly int[] DiasPorEtapa = { 0, 1, 3, 7, 14, 30 };

    public static int EtapaMaxima => DiasPorEtapa.Length - 1;

    public ResultadoAlgoritmo Calcular(int etapaActual, bool acerto, DateTime ahora, IReadOnlyList<bool> ultimasRespuestas)
    {
        if (!acerto)
        {
            return new ResultadoAlgoritmo(0, ahora);
        }

        var nuevaEtapa = Math.Min(etapaActual + 1, EtapaMaxima);
        return new ResultadoAlgoritmo(nuevaEtapa, ahora.AddDays(DiasPorEtapa[nuevaEtapa]));
    }
}
