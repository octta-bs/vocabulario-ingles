using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Aquí se registran las piezas que la aplicación va a usar.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Patrón Strategy: hoy se usan intervalos adaptativos (las palabras que fallas seguido vuelven antes).
// Para volver a intervalos fijos, o cambiar a FSRS más adelante, solo hay que cambiar esta línea
// por otra clase que implemente IAlgoritmoRepaso.
builder.Services.AddScoped<IAlgoritmoRepaso, IntervalosAdaptativos>();

// Patrón Strategy: regla para decidir cuántas palabras nuevas te tocan según tus aciertos.
builder.Services.AddScoped<IRitmoPalabrasNuevas, RitmoPorAciertos>();
builder.Services.AddSingleton<GeneradorEjercicios>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ZonaHorariaUsuario>();
builder.Services.AddScoped<RepasoService>();
builder.Services.AddScoped<EstadisticasService>();

var app = builder.Build();

// Crea o actualiza la base de datos (un archivo SQLite) con las migraciones, sin perder el progreso
// (antes de migrar hace un respaldo), y carga las palabras iniciales la primera vez.
using (var alcance = app.Services.CreateScope())
{
    var db = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
    PreparadorBaseDatos.Preparar(db, app.Logger);
    SeedData.Cargar(db);
}

// 2. Aquí se arma la cadena de pasos por la que pasa cada petición.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
