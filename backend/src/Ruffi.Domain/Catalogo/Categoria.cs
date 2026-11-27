namespace Ruffi.Domain.Catalogo;

public class Categoria
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Slug { get; set; }
    public int Orden { get; set; }
    public bool Activa { get; set; } = true;
}
