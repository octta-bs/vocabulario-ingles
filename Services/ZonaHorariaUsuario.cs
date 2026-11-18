namespace VocabularioIngles.Services;

/// <summary>
/// Convierte fechas UTC (como se guardan) a días de tu zona horaria, para contar "hoy", las rachas
/// y el progreso por día con la hora de México. La zona se configura en appsettings.json ("ZonaHoraria").
/// </summary>
public class ZonaHorariaUsuario
{
    private readonly TimeZoneInfo _zona;
    private readonly TimeProvider _reloj;

    public ZonaHorariaUsuario(IConfiguration configuracion, TimeProvider reloj)
    {
        _zona = Buscar(configuracion["ZonaHoraria"]);
        _reloj = reloj;
    }

    public DateTime AhoraUtc => _reloj.GetUtcNow().UtcDateTime;

    /// <summary>El día de hoy en tu zona horaria.</summary>
    public DateOnly Hoy => DiaLocal(AhoraUtc);

    /// <summary>En qué día (de tu zona horaria) cae una fecha UTC.</summary>
    public DateOnly DiaLocal(DateTime fechaUtc)
    {
        var utc = DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc, _zona));
    }

    /// <summary>La medianoche de un día de tu zona horaria, convertida a UTC.</summary>
    public DateTime InicioDelDiaUtc(DateOnly dia)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dia.ToDateTime(TimeOnly.MinValue), _zona);
    }

    public DateTime InicioDeHoyUtc() => InicioDelDiaUtc(Hoy);

    private static TimeZoneInfo Buscar(string? id)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(id) ? "America/Mexico_City" : id);
        }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.Local;
        }
    }
}
