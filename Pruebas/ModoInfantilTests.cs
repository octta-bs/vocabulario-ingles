using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class ModoInfantilTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly JuegoInfantilService _juego;

    public ModoInfantilTests()
    {
        _juego = new JuegoInfantilService(_base.Db, BaseDePrueba.Zona(TimeProvider.System));
    }

    public void Dispose() => _base.Dispose();

    private int CategoriaId(string nombre) => _base.Db.Categorias.Single(c => c.Nombre == nombre).Id;

    private int PalabraId(string ingles) => _base.Db.Palabras.Single(p => p.Ingles == ingles).Id;

    [Fact]
    public void Cada_palabra_de_las_categorias_infantiles_tiene_dibujo()
    {
        var sinDibujo = _base.Db.Palabras
            .Where(p => DibujosInfantiles.Categorias.Contains(p.Categoria!.Nombre))
            .AsEnumerable()
            .Where(p => !DibujosInfantiles.Existe(p.Ingles))
            .Select(p => p.Ingles)
            .ToList();

        Assert.Empty(sinDibujo);
        Assert.Equal(48, _base.Db.Palabras.Count(p => DibujosInfantiles.Categorias.Contains(p.Categoria!.Nombre)));
    }

    [Fact]
    public void Los_dibujos_son_svg_y_no_hay_dibujo_para_palabras_ajenas()
    {
        Assert.StartsWith("<svg", DibujosInfantiles.Svg("dog"));
        Assert.StartsWith("<svg", DibujosInfantiles.Svg("Red"));
        Assert.Null(DibujosInfantiles.Svg("database"));
    }

    [Fact]
    public async Task Muestra_las_4_categorias_en_orden()
    {
        var categorias = await _juego.CategoriasAsync();

        Assert.Equal(DibujosInfantiles.Categorias, categorias.Select(c => c.Nombre).ToArray());
        Assert.All(categorias, c => Assert.Equal(12, c.Total));
        Assert.All(categorias, c => Assert.Equal(0, c.Aprendidas));
    }

    [Fact]
    public async Task El_juego_tiene_8_rondas_con_3_opciones_de_la_misma_categoria()
    {
        var animales = CategoriaId("Animales");
        var idsAnimales = _base.Db.Palabras.Where(p => p.CategoriaId == animales).Select(p => p.Id).ToHashSet();

        for (var semilla = 0; semilla < 10; semilla++)
        {
            var rondas = await _juego.ArmarJuegoAsync(animales, new Random(semilla));

            Assert.Equal(JuegoInfantilService.RondasPorJuego, rondas.Count);
            Assert.All(rondas, r =>
            {
                Assert.Equal(3, r.Opciones.Count);
                Assert.Equal(3, r.Opciones.Select(o => o.Id).Distinct().Count());
                Assert.Contains(r.Opciones, o => o.Id == r.PalabraId);
                Assert.All(r.Opciones, o => Assert.Contains(o.Id, idsAnimales));
            });
            // En 8 rondas con 12 palabras no se repite ninguna.
            Assert.Equal(8, rondas.Select(r => r.PalabraId).Distinct().Count());
        }
    }

    [Fact]
    public async Task Primero_salen_las_que_fallo_y_al_final_las_aprendidas()
    {
        var colores = CategoriaId("Colores");
        await _juego.RegistrarAsync(PalabraId("purple"), false);
        foreach (var color in new[] { "red", "blue", "green", "yellow", "black", "white", "orange", "pink", "brown", "gray" })
        {
            for (var i = 0; i < ProgresoInfantil.AciertosParaAprender; i++)
            {
                await _juego.RegistrarAsync(PalabraId(color), true);
            }
        }

        var rondas = await _juego.ArmarJuegoAsync(colores, new Random(1));

        Assert.Equal(PalabraId("purple"), rondas[0].PalabraId);     // la que falló
        Assert.Equal(PalabraId("color"), rondas[1].PalabraId);      // la que nunca jugó
    }

    [Fact]
    public async Task Tres_aciertos_seguidos_la_dan_por_aprendida_y_un_fallo_reinicia()
    {
        var perro = PalabraId("dog");

        await _juego.RegistrarAsync(perro, true);
        await _juego.RegistrarAsync(perro, true);
        await _juego.RegistrarAsync(perro, false);
        await _juego.RegistrarAsync(perro, true);
        await _juego.RegistrarAsync(perro, true);
        Assert.Equal(0, (await _juego.CategoriaAsync(CategoriaId("Animales")))!.Aprendidas);

        await _juego.RegistrarAsync(perro, true);
        var progreso = _base.Db.ProgresosInfantiles.Single(p => p.PalabraId == perro);
        Assert.Equal((5, 1, 3), (progreso.Aciertos, progreso.Fallos, progreso.AciertosSeguidos));
        Assert.Equal(1, (await _juego.CategoriaAsync(CategoriaId("Animales")))!.Aprendidas);
        Assert.True((await _juego.PalabrasAsync(CategoriaId("Animales"))).Single(p => p.Id == perro).Aprendida);
    }

    [Fact]
    public async Task No_toca_el_progreso_del_adulto_ni_acepta_palabras_de_otras_categorias()
    {
        await _juego.RegistrarAsync(PalabraId("dog"), true);

        Assert.False(await _juego.RegistrarAsync(PalabraId("database"), true));
        Assert.False(await _juego.RegistrarAsync(999_999, true));
        Assert.Empty(_base.Db.Progresos);
        Assert.Empty(_base.Db.Respuestas);
        Assert.Single(_base.Db.ProgresosInfantiles);
        Assert.Empty(await _juego.ArmarJuegoAsync(CategoriaId("Tecnología"), new Random(1)));
    }

    [Fact]
    public async Task Guarda_el_nombre_del_nino_recortado()
    {
        Assert.Equal("", await _juego.ObtenerNombreAsync());

        await _juego.GuardarNombreAsync("  Ana  ");
        await _juego.GuardarNombreAsync("  Ana María  ");

        Assert.Equal("Ana María", await _juego.ObtenerNombreAsync());
        Assert.Single(_base.Db.PerfilesInfantiles);
    }
}
