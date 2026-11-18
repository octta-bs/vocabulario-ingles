namespace VocabularioIngles.Services;

/// <summary>
/// Decide las palabras nuevas según tu porcentaje de aciertos en las últimas 50 respuestas:
/// 90% o más → 8, de 75 a 89% → 5, de 60 a 74% → 3 y menos de 60% → 0 (primero refuerza).
/// Con menos de 10 respuestas todavía no hay con qué medir, y se dan 5 como antes.
/// </summary>
public class RitmoPorAciertos : IRitmoPalabrasNuevas
{
    public const int MinimoRespuestas = 10;
    public const int CantidadInicial = 5;

    public int RespuestasQueUsa => 50;

    public RitmoNuevas Calcular(IReadOnlyList<bool> ultimasRespuestas)
    {
        var respuestas = ultimasRespuestas.Take(RespuestasQueUsa).ToList();
        if (respuestas.Count < MinimoRespuestas)
        {
            return new RitmoNuevas(CantidadInicial, null);
        }

        var porcentaje = (int)Math.Floor(100.0 * respuestas.Count(a => a) / respuestas.Count);
        var cantidad = porcentaje switch
        {
            >= 90 => 8,
            >= 75 => 5,
            >= 60 => 3,
            _ => 0
        };

        return new RitmoNuevas(cantidad, porcentaje);
    }
}
