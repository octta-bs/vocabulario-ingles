namespace VocabularioIngles.Models;

/// <summary>Una categoría y cuántas de sus palabras todavía no entran al repaso.</summary>
public record CategoriaSinEmpezar(int Id, string Nombre, int SinEmpezar);
