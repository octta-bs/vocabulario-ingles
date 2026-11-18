using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class RespuestasPropiasTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly RespuestasPropiasService _servicio;

    public RespuestasPropiasTests()
    {
        _servicio = new RespuestasPropiasService(_base.Db, BaseDePrueba.Zona(TimeProvider.System));
    }

    public void Dispose() => _base.Dispose();

    private const string Clave = "tell-me-about-yourself";

    [Fact]
    public async Task Guarda_y_lee_una_respuesta()
    {
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = "  I'm Alex, a .NET developer.  " });

        var mias = await _servicio.ObtenerAsync();

        Assert.Equal("I'm Alex, a .NET developer.", mias[Clave]);
    }

    [Fact]
    public async Task Actualiza_la_respuesta_sin_duplicarla()
    {
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = "Primera versión" });
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = "Segunda versión" });

        Assert.Single(_base.Db.RespuestasPropias);
        Assert.Equal("Segunda versión", (await _servicio.ObtenerAsync())[Clave]);
    }

    [Fact]
    public async Task Una_respuesta_vacia_se_borra()
    {
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = "Algo" });
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = "   " });

        Assert.Empty(_base.Db.RespuestasPropias);
    }

    [Fact]
    public async Task Ignora_claves_que_no_son_de_ninguna_pregunta()
    {
        await _servicio.GuardarAsync(new Dictionary<string, string?> { ["inventada"] = "Hola" });

        Assert.Empty(_base.Db.RespuestasPropias);
    }

    [Fact]
    public async Task Recorta_el_texto_al_largo_maximo()
    {
        await _servicio.GuardarAsync(new Dictionary<string, string?> { [Clave] = new string('a', RespuestaPropia.LargoMaximo + 50) });

        Assert.Equal(RespuestaPropia.LargoMaximo, (await _servicio.ObtenerAsync())[Clave].Length);
    }

    [Fact]
    public void Las_claves_de_las_preguntas_son_unicas()
    {
        var claves = PreguntasEntrevista.Todas.Select(p => p.Clave).ToList();

        Assert.Equal(claves.Count, claves.Distinct().Count());
        Assert.All(claves, c => Assert.Matches("^[a-z0-9-]+$", c));
    }
}
