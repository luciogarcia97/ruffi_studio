using Microsoft.Extensions.Configuration;
using Ruffi.Infrastructure.Configuracion;

namespace Ruffi.Tests;

public class ConfiguracionTests
{
    [Fact]
    public void Lee_el_env_ignorando_comentarios_y_comillas()
    {
        var valores = ArchivoEnv.Leer(
        [
            "# comentario",
            "",
            "MYSQL_USER=ruffi",
            "export MYSQL_PORT = 3316",
            "MYSQL_PASSWORD=\"a=b;c\"",
            "sin_igual",
        ]).ToList();

        Assert.Equal([("MYSQL_USER", "ruffi"), ("MYSQL_PORT", "3316"), ("MYSQL_PASSWORD", "a=b;c")], valores);
    }

    [Fact]
    public void Arma_la_conexion_con_las_variables_mysql()
    {
        var conexion = ConexionMySql.Crear(Config(new()
        {
            ["MYSQL_HOST"] = "127.0.0.1", ["MYSQL_PORT"] = "3316", ["MYSQL_DATABASE"] = "ruffi",
            ["MYSQL_USER"] = "ruffi", ["MYSQL_PASSWORD"] = "a;b",
        }));

        Assert.Contains("port=3316", conexion, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("password=\"a;b\"", conexion, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void La_conexion_completa_tiene_prioridad() =>
        Assert.Equal("Server=x", ConexionMySql.Crear(Config(new() { ["ConnectionStrings:Ruffi"] = "Server=x", ["MYSQL_USER"] = "otro" })));

    [Fact]
    public void Sin_clave_explica_que_falta()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ConexionMySql.Crear(Config(new() { ["MYSQL_DATABASE"] = "ruffi", ["MYSQL_USER"] = "ruffi" })));
        Assert.Contains("MYSQL_PASSWORD", error.Message);
    }

    private static IConfiguration Config(Dictionary<string, string?> valores) =>
        new ConfigurationBuilder().AddInMemoryCollection(valores).Build();
}
