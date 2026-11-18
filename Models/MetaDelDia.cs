namespace VocabularioIngles.Models;

/// <summary>Tu meta de ejercicios para hoy y cuántos llevas.</summary>
public record MetaDelDia(int Meta, int Respondidas)
{
    public bool Cumplida => Respondidas >= Meta;
    public int Faltan => Math.Max(0, Meta - Respondidas);
    public int Porcentaje => Meta <= 0 ? 100 : Math.Min(100, Respondidas * 100 / Meta);
}
