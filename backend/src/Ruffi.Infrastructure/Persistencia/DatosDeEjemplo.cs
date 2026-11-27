using Microsoft.EntityFrameworkCore;
using Ruffi.Domain.Catalogo;
using Ruffi.Domain.Entregas;

namespace Ruffi.Infrastructure.Persistencia;

/// <summary>
/// Productos y precios inventados, solo para desarrollo.
/// Las fotos viven en frontend/public/demo, que no se sube al repositorio.
/// </summary>
public static class DatosDeEjemplo
{
    public static async Task CargarAsync(RuffiDbContext db, CancellationToken ct = default)
    {
        if (await db.Productos.AnyAsync(ct) || await db.Categorias.AnyAsync(ct)) return;

        var calendarios = new Categoria { Nombre = "Calendarios", Slug = "calendarios", Orden = 1, Descripcion = "De pared o de escritorio, anillados y con el diseño que elijas." };
        var fiestas = new Categoria { Nombre = "Fiestas infantiles", Slug = "fiestas-infantiles", Orden = 2, Descripcion = "Piñatas, bolsitas, guirnaldas y carteles con la temática del cumple." };

        db.Productos.AddRange(
            new Producto
            {
                Categoria = calendarios, Nombre = "Calendario de pared temático", Slug = "calendario-de-pared-tematico", Orden = 1,
                Descripcion = "Doce meses con el personaje que elijas, anillado y listo para colgar.", DiasElaboracion = 7,
                Variantes = [new() { Nombre = "A4", Precio = 12000, Orden = 1 }, new() { Nombre = "A3", Precio = 18000, Orden = 2 }],
                Imagenes =
                [
                    new() { Url = "/demo/calendario-pared.webp", TextoAlternativo = "Calendario anillado colgado en la pared, abierto en diciembre", Orden = 1 },
                    new() { Url = "/demo/calendario-colgado.webp", TextoAlternativo = "Una persona anota en el calendario colgado", Orden = 2 },
                ],
                Opciones = [new() { Nombre = "Personaje o tema", Ayuda = "Contanos cuál querés.", Tipo = TipoOpcion.Texto, Obligatoria = true, Orden = 1 }],
            },
            new Producto
            {
                Categoria = calendarios, Nombre = "Calendario con tus fotos", Slug = "calendario-con-tus-fotos", Orden = 2,
                Descripcion = "Tu mascota, tu familia o tus viajes en cada mes.", DiasElaboracion = 10,
                Variantes = [new() { Nombre = "A4", Precio = 16000 }],
                Opciones =
                [
                    new() { Nombre = "Fotos", Ayuda = "Coordinamos el envío de las fotos después del pedido.", Tipo = TipoOpcion.ArchivosExternos, Obligatoria = true, Orden = 1 },
                    new() { Nombre = "Texto de tapa", Ayuda = "Por ejemplo, un nombre.", Tipo = TipoOpcion.Texto, Orden = 2 },
                ],
            },
            new Producto
            {
                Categoria = fiestas, Nombre = "Piñata temática", Slug = "pinata-tematica", Orden = 3,
                Descripcion = "Hecha a mano con cartón y papel, del personaje de la fiesta.", DiasElaboracion = 10,
                Variantes = [new() { Nombre = "Mediana", Precio = 25000, Orden = 1 }, new() { Nombre = "Grande", Precio = 38000, Orden = 2 }],
                Imagenes = [new() { Url = "/demo/pinata.webp", TextoAlternativo = "Piñata con forma de personajes de bloques", Orden = 1 }],
                Opciones = [new() { Nombre = "Personaje o tema", Ayuda = "Contanos cuál querés.", Tipo = TipoOpcion.Texto, Obligatoria = true, Orden = 1 }],
            },
            new Producto
            {
                Categoria = fiestas, Nombre = "Bolsitas de golosinas", Slug = "bolsitas-de-golosinas", Orden = 4,
                Descripcion = "Bolsitas de papel con etiqueta personalizada con el nombre y la edad.", DiasElaboracion = 5,
                Variantes = [new() { Nombre = "Pack de 10", Precio = 9000, Orden = 1 }, new() { Nombre = "Pack de 20", Precio = 16000, Orden = 2 }],
                Imagenes = [new() { Url = "/demo/mesa-dulce.webp", TextoAlternativo = "Mesa de cumpleaños con bolsitas, piñatas y torta", Orden = 1 }],
                Opciones = [new() { Nombre = "Nombre y edad", Ayuda = "Así aparecen en la etiqueta.", Tipo = TipoOpcion.Texto, Obligatoria = true, Orden = 1 }],
            },
            new Producto
            {
                Categoria = fiestas, Nombre = "Guirnalda de feliz cumple", Slug = "guirnalda-de-feliz-cumple", Orden = 5,
                Descripcion = "Banderines con letras y fotos, con el nombre del cumpleañero.", DiasElaboracion = 5,
                Variantes = [new() { Nombre = "Única", Precio = 8000 }],
                Imagenes = [new() { Url = "/demo/guirnalda.webp", TextoAlternativo = "Guirnalda de feliz cumple colgada en una ventana", Orden = 1 }],
                Opciones = [new() { Nombre = "Nombre", Ayuda = "Va en los banderines.", Tipo = TipoOpcion.Texto, Obligatoria = true, Orden = 1 }],
            });
        db.MetodosEntrega.AddRange(
            new MetodoEntrega { Nombre = "Retiro", Tipo = TipoEntrega.Retiro, Orden = 1 },
            new MetodoEntrega { Nombre = "Envío", Tipo = TipoEntrega.Envio, Costo = 5000, GratisDesde = 40000, Orden = 2 });
        await db.SaveChangesAsync(ct);
    }
}
