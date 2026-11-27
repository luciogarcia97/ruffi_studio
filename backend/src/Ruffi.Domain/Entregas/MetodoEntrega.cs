namespace Ruffi.Domain.Entregas;

public enum TipoEntrega
{
    Retiro,
    Envio,
}

public class MetodoEntrega
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public TipoEntrega Tipo { get; set; }
    public decimal Costo { get; set; }

    /// <summary>Subtotal de productos desde el cual el envío se bonifica.</summary>
    public decimal? GratisDesde { get; set; }

    public bool Activo { get; set; } = true;
    public int Orden { get; set; }

    public decimal CalcularCosto(decimal subtotal)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(subtotal);
        return Tipo == TipoEntrega.Retiro || subtotal >= GratisDesde ? 0 : Costo;
    }
}
