using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ruffi.Application.Catalogo;
using Ruffi.Infrastructure.Catalogo;
using Ruffi.Infrastructure.Persistencia;

namespace Ruffi.Infrastructure;

public static class Dependencias
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string conexion)
    {
        services.AddDbContext<RuffiDbContext>(o => o.UseMySQL(conexion));
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICatalogo, CatalogoService>();
        return services;
    }
}
