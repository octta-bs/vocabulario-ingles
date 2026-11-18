namespace VocabularioIngles.Models;

/// <summary>Lo que muestra la pantalla de inicio.</summary>
public class InicioViewModel
{
    public string Saludo { get; init; } = "";
    public string Fecha { get; init; } = "";
    public MetaDelDia Meta { get; init; } = new(Configuracion.MetaPorDefecto, 0);
    public int Racha { get; init; }
    public int Pendientes { get; init; }

    // Cuántas palabras nuevas te tocan ahora (ya limitado a las que quedan sin empezar).
    public int NuevasPorAgregar { get; init; }
    public int Errores { get; init; }
}

/// <summary>Lo que muestra la pantalla "Practicar".</summary>
public class PracticarViewModel
{
    public int RespuestasEscritas { get; init; }
    public int TotalPreguntas { get; init; }
    public int Errores { get; init; }
    public int TotalPalabras { get; init; }
    public int Categorias { get; init; }
}
