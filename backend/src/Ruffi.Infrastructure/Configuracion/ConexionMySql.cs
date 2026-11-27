using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Ruffi.Infrastructure.Configuracion;

public static class ConexionMySql
{
    /// <summary>Usa ConnectionStrings:Ruffi si está definida; si no, la arma con las variables MYSQL_*.</summary>
    public static string Crear(IConfiguration config)
    {
        if (config.GetConnectionString("Ruffi") is { Length: > 0 } completa) return completa;

        string Requerida(string clave) => config[clave] is { Length: > 0 } v
            ? v
            : throw new InvalidOperationException($"Falta {clave}. Copiá .env.example como .env y completalo.");

        return new MySqlConnectionStringBuilder
        {
            Server = config["MYSQL_HOST"] ?? "127.0.0.1",
            Port = uint.Parse(config["MYSQL_PORT"] ?? "3306"),
            Database = Requerida("MYSQL_DATABASE"),
            UserID = Requerida("MYSQL_USER"),
            Password = Requerida("MYSQL_PASSWORD"),
        }.ConnectionString;
    }
}
