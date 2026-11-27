namespace Ruffi.Application.Catalogo;

public interface ICatalogo
{
    /// <summary>Solo lo que hoy se puede vender.</summary>
    Task<CatalogoDto> ObtenerAsync(CancellationToken ct);
}

/// <summary>Contrato público: lo usan la API y el JSON exportado.</summary>
public record CatalogoDto(IReadOnlyList<ProductoDto> Productos, IReadOnlyList<MetodoEntregaDto> Entregas);

public record ProductoDto(
    int Id, string Nombre, string Slug, string Descripcion, string Categoria, int DiasElaboracion,
    IReadOnlyList<VarianteDto> Variantes, IReadOnlyList<ImagenDto> Imagenes, IReadOnlyList<OpcionDto> Opciones);

public record VarianteDto(int Id, string Nombre, decimal Precio);
public record ImagenDto(string Url, string TextoAlternativo);
public record OpcionDto(int Id, string Nombre, string Ayuda, string Tipo, bool Obligatoria, decimal Recargo);
public record MetodoEntregaDto(int Id, string Nombre, string Tipo, decimal Costo, decimal? GratisDesde);
