using Microsoft.EntityFrameworkCore;
using Ruffi.Application.Catalogo;
using Ruffi.Infrastructure;
using Ruffi.Infrastructure.Persistencia;

var builder = WebApplication.CreateBuilder(args);

var conexion = builder.Configuration.GetConnectionString("Ruffi")
    ?? throw new InvalidOperationException("Falta la conexión ConnectionStrings:Ruffi.");
builder.Services.AddInfrastructure(conexion);
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
