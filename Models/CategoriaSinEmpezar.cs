namespace VocabularioIngles.Models;

/// <summary>Una categoría, su nivel y cuántas de sus palabras todavía no entran al repaso.</summary>
public record CategoriaSinEmpezar(int Id, string Nombre, NivelMcer Nivel, int SinEmpezar);
