namespace Ruffi.Infrastructure.Configuracion;

/// <summary>
/// Carga un archivo .env en las variables de entorno del proceso.
/// Solo para desarrollo: en un servidor las variables se definen en el entorno.
/// </summary>
public static class ArchivoEnv
{
    /// <summary>Busca .env desde <paramref name="desde"/> hacia arriba. Las variables ya definidas no se pisan.</summary>
    public static string? Cargar(string desde)
    {
        for (var dir = new DirectoryInfo(desde); dir is not null; dir = dir.Parent)
        {
            var ruta = Path.Combine(dir.FullName, ".env");
            if (!File.Exists(ruta)) continue;

            foreach (var (clave, valor) in Leer(File.ReadAllLines(ruta)))
                if (Environment.GetEnvironmentVariable(clave) is null)
                    Environment.SetEnvironmentVariable(clave, valor);
            return ruta;
        }
        return null;
    }

    public static IEnumerable<(string Clave, string Valor)> Leer(IEnumerable<string> lineas)
    {
        foreach (var cruda in lineas)
        {
            var linea = cruda.Trim();
            if (linea.Length == 0 || linea.StartsWith('#')) continue;
            if (linea.StartsWith("export ")) linea = linea["export ".Length..].TrimStart();

            var igual = linea.IndexOf('=');
            if (igual <= 0) continue;

            var clave = linea[..igual].Trim();
            var valor = linea[(igual + 1)..].Trim();
            if (valor.Length >= 2 && (valor[0] == '"' || valor[0] == '\'') && valor[^1] == valor[0])
                valor = valor[1..^1];
            yield return (clave, valor);
        }
    }
}
