namespace VocabularioIngles.Models;

/// <summary>Cuántos ejercicios respondiste un día y cuántos acertaste.</summary>
public record DiaDeProgreso(DateOnly Dia, int Ejercicios, int Aciertos)
{
    public int? Porcentaje => Ejercicios == 0 ? null : Aciertos * 100 / Ejercicios;
}

/// <summary>Una palabra y cuántas veces la has fallado y acertado.</summary>
public record PalabraDificil(Palabra Palabra, int Fallos, int Aciertos);

/// <summary>Todo lo que muestra la pantalla de progreso.</summary>
public class ResumenProgreso
{
    public int Meta { get; init; }

    // Días seguidos cumpliendo la meta. Si hoy todavía no la cumples, la racha cuenta hasta ayer.
    public int RachaActual { get; init; }
    public int MejorRacha { get; init; }
    public bool HoyCumplida { get; init; }

    public int TotalRespuestas { get; init; }
    public int? PorcentajeTotal { get; init; }

    // Los últimos días, del más viejo a hoy (incluye los días sin práctica).
    public List<DiaDeProgreso> UltimosDias { get; init; } = new();
    public List<PalabraDificil> MasFalladas { get; init; } = new();
}
