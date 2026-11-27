using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Ruffi.Application.Catalogo;

namespace Ruffi.Tests;

public class ApiTests(WebApplicationFactory<Program> fabrica) : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient Cliente() => fabrica.WithWebHostBuilder(b =>
    {
        b.UseEnvironment("Testing");
        b.UseSetting("ConnectionStrings:Ruffi", "Server=no-se-usa");
        b.ConfigureTestServices(s => s.AddScoped<ICatalogo, CatalogoFalso>());
    }).CreateClient();

    [Fact]
    public async Task Devuelve_el_catalogo()
    {
        var c = await Cliente().GetFromJsonAsync<CatalogoDto>("/api/catalogo");
        Assert.Equal("Envio", Assert.Single(c!.Entregas).Tipo);
    }

    [Fact]
    public async Task Ruta_inexistente_devuelve_404() =>
        Assert.Equal(HttpStatusCode.NotFound, (await Cliente().GetAsync("/api/nada")).StatusCode);

    private class CatalogoFalso : ICatalogo
    {
        public Task<CatalogoDto> ObtenerAsync(CancellationToken ct) =>
            Task.FromResult(new CatalogoDto([], [], [new(1, "Envío", "Envio", 5000, 40000)]));
    }
}
