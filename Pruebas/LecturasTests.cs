using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class LecturasTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly LecturasService _lecturas;

    public LecturasTests()
    {
        _lecturas = new LecturasService(_base.Db, BaseDePrueba.Zona(TimeProvider.System));
    }

    public void Dispose() => _base.Dispose();

    private const string Texto = "Run the migrations before you deploy. Don't skip the tests!\nThe pipeline is flaky.";

    [Fact]
    public void Parte_el_texto_en_palabras_y_oraciones()
    {
        var (segmentos, oraciones) = LecturasService.Partir(Texto, new HashSet<string> { "deploy" });

        Assert.Equal(new[] { "Run the migrations before you deploy.", "Don't skip the tests!", "The pipeline is flaky." }, oraciones);
        var palabras = segmentos.Where(s => s.EsPalabra).ToList();
        Assert.Contains(palabras, s => s.Texto == "Don't" && s.Oracion == 1);
        Assert.Equal(2, palabras.Single(s => s.Texto == "flaky").Oracion);
        Assert.True(palabras.Single(s => s.Texto == "deploy").YaLaAprendes);
        Assert.False(palabras.Single(s => s.Texto == "migrations").YaLaAprendes);
        // Unir los segmentos devuelve el texto original, sin perder nada.
        Assert.Equal(Texto, string.Concat(segmentos.Select(s => s.Texto)));
    }

    [Fact]
    public async Task Crear_lectura_sin_titulo_usa_las_primeras_palabras()
    {
        var lectura = await _lecturas.CrearAsync("  ", Texto);

        Assert.Equal("Run the migrations before you deploy.", lectura!.Titulo);
        Assert.Null(await _lecturas.CrearAsync("Vacía", "   "));
    }

    [Fact]
    public async Task Una_palabra_nueva_entra_al_repaso_con_su_frase()
    {
        var resultado = await _lecturas.GuardarPalabraAsync("Flaky", "The pipeline is flaky.", "inestable");

        Assert.True(resultado.Guardada);
        var palabra = _base.Db.Palabras.Include(p => p.Categoria).Include(p => p.Progreso).Single(p => p.Ingles == "flaky");
        Assert.Equal("inestable", palabra.Espanol);
        Assert.Equal("The pipeline is flaky.", palabra.Ejemplo);
        Assert.Equal(LecturasService.NombreCategoria, palabra.Categoria!.Nombre);
        Assert.Equal(NivelMcer.B1, palabra.Categoria.Nivel);
        Assert.Equal(ModoApp.Trabajo, Modos.De(palabra.Categoria.Nombre));
        Assert.NotNull(palabra.Progreso);
        // Con la frase como ejemplo tiene ejercicio de completar.
        Assert.Contains(new GeneradorEjercicios().Posibles(palabra), e => e.Tipo == TipoEjercicio.CompletarFrase);
    }

    [Fact]
    public async Task Una_palabra_que_ya_existe_no_se_duplica()
    {
        Assert.Equal("código", await _lecturas.BuscarSignificadoAsync("Code"));

        var primera = await _lecturas.GuardarPalabraAsync("code", "Read the code.", "");
        var segunda = await _lecturas.GuardarPalabraAsync("code", "Read the code.", "");

        Assert.True(primera.Guardada);
        Assert.Contains("ya existía", primera.Mensaje);
        Assert.Contains("ya está en tu repaso", segunda.Mensaje);
        Assert.Single(_base.Db.Palabras.Where(p => p.Ingles == "code"));
        Assert.Single(_base.Db.Progresos);
    }

    [Theory]
    [InlineData("flaky", "")]
    [InlineData("two words", "algo")]
    [InlineData("123", "algo")]
    public async Task Rechaza_palabras_invalidas_o_sin_significado(string palabra, string significado)
    {
        var resultado = await _lecturas.GuardarPalabraAsync(palabra, "The pipeline is flaky.", significado);

        Assert.False(resultado.Guardada);
        Assert.Empty(_base.Db.Progresos);
    }

    [Fact]
    public async Task Si_la_frase_no_contiene_la_palabra_se_guarda_sin_ejemplo()
    {
        await _lecturas.GuardarPalabraAsync("rollback", "Something unrelated.", "reversión");

        Assert.Equal("", _base.Db.Palabras.Single(p => p.Ingles == "rollback").Ejemplo);
    }

    [Fact]
    public async Task Borrar_la_lectura_conserva_las_palabras()
    {
        var lectura = await _lecturas.CrearAsync("Notas", Texto);
        await _lecturas.GuardarPalabraAsync("flaky", "The pipeline is flaky.", "inestable");

        Assert.True(await _lecturas.BorrarAsync(lectura!.Id));
        Assert.Empty(_base.Db.Lecturas);
        Assert.Single(_base.Db.Palabras.Where(p => p.Ingles == "flaky"));
    }
}
