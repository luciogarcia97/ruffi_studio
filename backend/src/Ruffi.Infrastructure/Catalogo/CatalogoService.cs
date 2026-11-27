using Microsoft.EntityFrameworkCore;
using Ruffi.Application.Catalogo;
using Ruffi.Infrastructure.Persistencia;

namespace Ruffi.Infrastructure.Catalogo;

public class CatalogoService(RuffiDbContext db, TimeProvider reloj) : ICatalogo
{
    public async Task<CatalogoDto> ObtenerAsync(CancellationToken ct)
    {
        var ahora = reloj.GetUtcNow().UtcDateTime;
        var productos = await db.Productos.AsNoTracking().AsSplitQuery()
            .Include(p => p.Categoria).Include(p => p.Variantes).Include(p => p.Imagenes).Include(p => p.Opciones)
            .Where(p => p.Activo)
            .OrderBy(p => p.Orden).ThenBy(p => p.Id)
            .ToListAsync(ct);
        var entregas = await db.MetodosEntrega.AsNoTracking()
            .Where(m => m.Activo).OrderBy(m => m.Orden).ThenBy(m => m.Id).ToListAsync(ct);

        return new CatalogoDto(
            [.. productos.Where(p => p.EstaDisponible(ahora)).Select(p => new ProductoDto(
                p.Id, p.Nombre, p.Slug, p.Descripcion, p.Categoria!.Nombre, p.DiasElaboracion,
                [.. p.Variantes.Where(v => v.Activa).OrderBy(v => v.Orden).ThenBy(v => v.Id).Select(v => new VarianteDto(v.Id, v.Nombre, v.Precio))],
                [.. p.Imagenes.OrderBy(i => i.Orden).ThenBy(i => i.Id).Select(i => new ImagenDto(i.Url, i.TextoAlternativo))],
                [.. p.Opciones.OrderBy(o => o.Orden).ThenBy(o => o.Id).Select(o => new OpcionDto(o.Id, o.Nombre, o.Ayuda, o.Tipo.ToString(), o.Obligatoria, o.Recargo))]))],
            [.. entregas.Select(m => new MetodoEntregaDto(m.Id, m.Nombre, m.Tipo.ToString(), m.Costo, m.GratisDesde))]);
    }
}
