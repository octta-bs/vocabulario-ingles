using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Aquí se registran las piezas que la aplicación va a usar.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Patrón Strategy: hoy se usan intervalos fijos. Para cambiar a FSRS más adelante
// solo hay que cambiar esta línea por otra clase que implemente IAlgoritmoRepaso.
builder.Services.AddScoped<IAlgoritmoRepaso, IntervalosFijos>();
builder.Services.AddSingleton<GeneradorEjercicios>();
builder.Services.AddScoped<RepasoService>();

var app = builder.Build();

// Crea la base de datos (un archivo SQLite) la primera vez y carga las palabras iniciales.
using (var alcance = app.Services.CreateScope())
{
    var db = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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
