using Microsoft.EntityFrameworkCore;
using Ruffi.Application.Catalogo;
using Ruffi.Infrastructure;
using Ruffi.Infrastructure.Configuracion;
using Ruffi.Infrastructure.Persistencia;

// En desarrollo las credenciales vienen de .env; en un servidor, del entorno.
ArchivoEnv.Cargar(Directory.GetCurrentDirectory());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(ConexionMySql.Crear(builder.Configuration));
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // En producción las migraciones se aplican como paso del despliegue.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RuffiDbContext>();
    await db.Database.MigrateAsync();
    await DatosDeEjemplo.CargarAsync(db);
}

app.MapGet("/api/salud", () => Results.Ok(new { estado = "ok" }));
app.MapGet("/api/catalogo", (ICatalogo catalogo, CancellationToken ct) => catalogo.ObtenerAsync(ct));

app.Run();

public partial class Program;
