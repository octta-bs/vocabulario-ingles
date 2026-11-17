namespace VocabularioIngles.Services;

/// <summary>
/// Igual que <see cref="IntervalosFijos"/>, pero las palabras difíciles vuelven antes.
/// Una palabra es "difícil" si fallaste 2 o más de sus últimas 5 respuestas; mientras lo sea,
/// los días de espera se reducen a la mitad (mínimo 1 día): 1, 1, 3, 7 y 15 en vez de 1, 3, 7, 14 y 30.
/// Cuando la aciertas varias veces seguidas, los fallos salen de la ventana y vuelve al ritmo normal.
/// </summary>
public class IntervalosAdaptativos : IAlgoritmoRepaso
{
    public const int Ventana = 5;
    public const int FallosParaDificil = 2;

    private readonly IntervalosFijos _normal = new();

    public static bool EsDificil(IReadOnlyList<bool> ultimasRespuestas)
    {
        return ultimasRespuestas.Take(Ventana).Count(acerto => !acerto) >= FallosParaDificil;
    }

    public ResultadoAlgoritmo Calcular(int etapaActual, bool acerto, DateTime ahora, IReadOnlyList<bool> ultimasRespuestas)
    {
        var resultado = _normal.Calcular(etapaActual, acerto, ahora, ultimasRespuestas);
        if (!acerto || !EsDificil(ultimasRespuestas))
        {
            return resultado;
        }

        var dias = (int)Math.Round((resultado.ProximoRepaso - ahora).TotalDays);
        var diasCortos = dias <= 0 ? 0 : Math.Max(1, dias / 2);
        return resultado with { ProximoRepaso = ahora.AddDays(diasCortos) };
    }
}
