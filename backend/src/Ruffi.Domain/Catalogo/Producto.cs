namespace Ruffi.Domain.Catalogo;

public class Producto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public required string Nombre { get; set; }
    public required string Slug { get; set; }
    public string Descripcion { get; set; } = "";
    public bool Activo { get; set; } = true;
    public int Orden { get; set; }

    /// <summary>Días estimados de elaboración: se fabrica a demanda.</summary>
    public int DiasElaboracion { get; set; }

    /// <summary>Inicio de venta en UTC, inclusive.</summary>
    public DateTime? VentaDesde { get; set; }

    /// <summary>Fin de venta en UTC, exclusivo.</summary>
    public DateTime? VentaHasta { get; set; }

    public List<Variante> Variantes { get; set; } = [];
    public List<Imagen> Imagenes { get; set; } = [];
    public List<OpcionPersonalizacion> Opciones { get; set; } = [];

    public bool EstaDisponible(DateTime ahoraUtc) =>
        Activo
        && Categoria is not { Activa: false }
        && (VentaDesde is null || VentaDesde <= ahoraUtc)
        && (VentaHasta is null || ahoraUtc < VentaHasta)
        && Variantes.Any(v => v.Activa);
}

public class Variante
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public bool Activa { get; set; } = true;
    public int Orden { get; set; }
}

public class Imagen
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public required string Url { get; set; }
    public string TextoAlternativo { get; set; } = "";
    public int Orden { get; set; }
}

public enum TipoOpcion
{
    Texto,

    /// <summary>El cliente manda archivos por fuera de la web.</summary>
    ArchivosExternos,
}

public class OpcionPersonalizacion
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public required string Nombre { get; set; }
    public string Ayuda { get; set; } = "";
    public TipoOpcion Tipo { get; set; }
    public bool Obligatoria { get; set; }
    public decimal Recargo { get; set; }
    public int Orden { get; set; }
}
