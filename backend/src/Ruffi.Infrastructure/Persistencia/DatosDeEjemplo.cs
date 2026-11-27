using Microsoft.EntityFrameworkCore;
using Ruffi.Domain.Catalogo;
using Ruffi.Domain.Entregas;

namespace Ruffi.Infrastructure.Persistencia;

/// <summary>Productos y precios inventados, solo para desarrollo.</summary>
public static class DatosDeEjemplo
{
    public static async Task CargarAsync(RuffiDbContext db, CancellationToken ct = default)
    {
        if (await db.Productos.AnyAsync(ct) || await db.Categorias.AnyAsync(ct)) return;

        var calendarios = new Categoria { Nombre = "Calendarios", Slug = "calendarios" };
        db.Productos.AddRange(
            new Producto
            {
                Categoria = calendarios, Nombre = "Calendario de pared", Slug = "calendario-de-pared", Orden = 1,
                Descripcion = "Doce meses para colgar, impreso y armado a mano.", DiasElaboracion = 7,
                Variantes = [new() { Nombre = "A4", Precio = 12000, Orden = 1 }, new() { Nombre = "A3", Precio = 18000, Orden = 2 }],
            },
            new Producto
            {
                Categoria = calendarios, Nombre = "Calendario con tus fotos", Slug = "calendario-con-tus-fotos", Orden = 2,
                Descripcion = "Tu gato, tu familia o lo que quieras en cada mes.", DiasElaboracion = 10,
                Variantes = [new() { Nombre = "A4", Precio = 16000 }],
                Opciones =
                [
                    new() { Nombre = "Fotos", Ayuda = "Coordinamos el envío de las fotos después del pedido.", Tipo = TipoOpcion.ArchivosExternos, Obligatoria = true, Orden = 1 },
                    new() { Nombre = "Texto de tapa", Ayuda = "Por ejemplo, el nombre de tu gato.", Tipo = TipoOpcion.Texto, Orden = 2 },
                ],
            },
            new Producto
            {
                Categoria = calendarios, Nombre = "Calendario de escritorio", Slug = "calendario-de-escritorio", Orden = 3,
                Descripcion = "Chiquito, con base para apoyar.", DiasElaboracion = 5,
                Variantes = [new() { Nombre = "Único", Precio = 9000 }],
            });
        db.MetodosEntrega.AddRange(
            new MetodoEntrega { Nombre = "Retiro", Tipo = TipoEntrega.Retiro, Orden = 1 },
            new MetodoEntrega { Nombre = "Envío", Tipo = TipoEntrega.Envio, Costo = 5000, GratisDesde = 40000, Orden = 2 });
        await db.SaveChangesAsync(ct);
    }
}
