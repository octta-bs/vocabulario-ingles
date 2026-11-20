namespace VocabularioIngles.Models;

/// <summary>Lo que muestra la pantalla de inicio.</summary>
public class InicioViewModel
{
    public VocabularioIngles.Services.ModoApp Modo { get; init; }

    // Pendientes del otro modo (Principiante o Trabajo), para avisar que también esperan.
    public int PendientesOtroModo { get; init; }

    public string Saludo { get; init; } = "";
    public string Fecha { get; init; } = "";
    public MetaDelDia Meta { get; init; } = new(Configuracion.MetaPorDefecto, 0);
    public int Racha { get; init; }
    public int Pendientes { get; init; }

    // Cuántas palabras nuevas te tocan ahora (ya limitado a las que quedan sin empezar).
    public int NuevasPorAgregar { get; init; }
    public int Errores { get; init; }

    // La ruta de aprendizaje: en qué etapa vas y qué sigue.
    public VocabularioIngles.Services.RutaEstado? Ruta { get; init; }
}

/// <summary>Lo que muestra la pantalla "Practicar".</summary>
public class PracticarViewModel
{
    public VocabularioIngles.Services.ModoApp Modo { get; init; }
    public int RespuestasEscritas { get; init; }
    public int TotalPreguntas { get; init; }
    public int Errores { get; init; }
    public int TotalPalabras { get; init; }
    public int Categorias { get; init; }

    // Gramática: cuántos temas hay y en cuántos ya elegiste "Practicar este tema".
    public int TotalTemasGramatica { get; init; }
    public int TemasGramaticaEmpezados { get; init; }

    // La práctica que se sugiere hoy (arriba de todo), o null si no hay ninguna en especial.
    public VocabularioIngles.Services.Recomendacion? Recomendacion { get; init; }
}

/// <summary>Una opción de la lista de Practicar (vista parcial _OpcionPracticar).</summary>
public record OpcionPracticar(string Controlador, string Accion, string Icono, string Nombre, string Detalle, string? Id = null);
