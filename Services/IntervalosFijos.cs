namespace VocabularioIngles.Services;

/// <summary>
/// Algoritmo sencillo: cada acierto sube una etapa y la palabra tarda más en volver.
/// Un error la regresa a la etapa 0.
/// </summary>
public class IntervalosFijos : IAlgoritmoRepaso
{
    // Días de espera según la etapa a la que se llega.
    // Las dos primeras etapas son "0 días": la palabra se repite el mismo día que se aprende
    // (práctica concentrada), y después se va espaciando: 1, 3, 7, 14 y 30 días.
    private static readonly int[] DiasPorEtapa = { 0, 0, 1, 3, 7, 14, 30 };

    public static int EtapaMaxima => DiasPorEtapa.Length - 1;

    public ResultadoAlgoritmo Calcular(int etapaActual, bool acerto, DateTime ahora)
    {
        if (!acerto)
        {
            return new ResultadoAlgoritmo(0, ahora);
        }

        var nuevaEtapa = Math.Min(etapaActual + 1, EtapaMaxima);
        return new ResultadoAlgoritmo(nuevaEtapa, ahora.AddDays(DiasPorEtapa[nuevaEtapa]));
    }
}
