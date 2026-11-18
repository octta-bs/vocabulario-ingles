using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class ModosTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly RepasoService _repaso;
    private readonly EstadisticasService _estadisticas;

    public ModosTests()
    {
        var zona = BaseDePrueba.Zona(TimeProvider.System);
        _repaso = new RepasoService(_base.Db, new IntervalosAdaptativos(), new GeneradorEjercicios(), new RitmoPorAciertos(), zona);
        _estadisticas = new EstadisticasService(_base.Db, zona);
    }

    public void Dispose() => _base.Dispose();

    private ModoApp ModoDe(int palabraId) =>
        Modos.De(_base.Db.Palabras.Include(p => p.Categoria).Single(p => p.Id == palabraId).Categoria!.Nombre);

    [Theory]
    [InlineData("Colores", ModoApp.Principiante)]
    [InlineData("Verbos comunes", ModoApp.Principiante)]
    [InlineData("Tecnología", ModoApp.Trabajo)]
    [InlineData("Stand-up", ModoApp.Trabajo)]
    [InlineData("Entrevista técnica", ModoApp.Trabajo)]
    public void Cada_categoria_pertenece_a_su_modo(string categoria, ModoApp esperado)
    {
        Assert.Equal(esperado, Modos.De(categoria));
    }

    [Fact]
    public async Task Cada_categoria_esta_en_un_solo_modo()
    {
        var principiante = await Modos.CategoriasAsync(_base.Db, ModoApp.Principiante);
        var trabajo = await Modos.CategoriasAsync(_base.Db, ModoApp.Trabajo);

        Assert.Equal(VocabularioGeneral.Categorias.Length, principiante.Count);
        Assert.Empty(principiante.Intersect(trabajo));
        Assert.Equal(_base.Db.Categorias.Count(), principiante.Count + trabajo.Count);
    }

    [Fact]
    public async Task En_modo_trabajo_las_nuevas_empiezan_por_tecnologia()
    {
        await _repaso.AgregarNuevasAsync(modo: ModoApp.Trabajo);

        var nuevas = _base.Db.Progresos.Include(p => p.Palabra).ThenInclude(p => p!.Categoria).ToList();
        Assert.Equal(5, nuevas.Count);
        Assert.All(nuevas, p => Assert.Equal("Tecnología", p.Palabra!.Categoria!.Nombre));
    }

    [Fact]
    public async Task Cada_modo_tiene_su_propio_repaso()
    {
        await _repaso.AgregarNuevasAsync(modo: ModoApp.Principiante);
        await _repaso.AgregarNuevasAsync(modo: ModoApp.Trabajo);

        Assert.Equal(5, await _repaso.ContarPendientesAsync(ModoApp.Principiante));
        Assert.Equal(5, await _repaso.ContarPendientesAsync(ModoApp.Trabajo));
        Assert.Equal(10, await _repaso.ContarPendientesAsync());

        var ejercicioPrincipiante = await _repaso.SiguienteEjercicioAsync(ModoApp.Principiante);
        var ejercicioTrabajo = await _repaso.SiguienteEjercicioAsync(ModoApp.Trabajo);
        Assert.Equal(ModoApp.Principiante, ModoDe(ejercicioPrincipiante!.Palabra.Id));
        Assert.Equal(ModoApp.Trabajo, ModoDe(ejercicioTrabajo!.Palabra.Id));
    }

    [Fact]
    public async Task Las_categorias_nuevas_y_los_errores_se_separan_por_modo()
    {
        var categorias = await _repaso.CategoriasConNuevasAsync(ModoApp.Trabajo);
        Assert.All(categorias, c => Assert.Equal(ModoApp.Trabajo, Modos.De(c.Nombre)));
        Assert.DoesNotContain(categorias, c => c.Nombre == "Colores");

        await _repaso.AgregarNuevasAsync(modo: ModoApp.Principiante);
        await _repaso.AgregarNuevasAsync(modo: ModoApp.Trabajo);
        var red = _base.Db.Palabras.Single(p => p.Ingles == "red").Id;
        var code = _base.Db.Palabras.Single(p => p.Ingles == "code").Id;
        await _repaso.ResponderAsync(red, TipoEjercicio.EscribirPalabra, "mal");
        await _repaso.ResponderAsync(code, TipoEjercicio.EscribirPalabra, "mal");

        Assert.Equal(new[] { red }, (await _estadisticas.MasFalladasAsync(10, ModoApp.Principiante)).Select(f => f.Palabra.Id));
        Assert.Equal(new[] { code }, (await _estadisticas.MasFalladasAsync(10, ModoApp.Trabajo)).Select(f => f.Palabra.Id));
        Assert.Equal(2, (await _estadisticas.MasFalladasAsync(10)).Count);
    }
}
