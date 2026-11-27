using Ruffi.Domain.Catalogo;
using Ruffi.Domain.Entregas;

namespace Ruffi.Tests;

public class ReglasTests
{
    private static readonly DateTime Ahora = new(2026, 12, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Producto Producto() => new()
    {
        Nombre = "Prueba", Slug = "prueba", Categoria = new() { Nombre = "C", Slug = "c" },
        Variantes = [new() { Nombre = "A4", Precio = 100 }],
    };

    [Fact]
    public void Sin_fechas_esta_disponible() => Assert.True(Producto().EstaDisponible(Ahora));

    [Fact]
    public void Inactivo_no_esta_disponible()
    {
        var p = Producto();
        p.Activo = false;
        Assert.False(p.EstaDisponible(Ahora));
    }

    [Fact]
    public void Categoria_inactiva_no_esta_disponible()
    {
        var p = Producto();
        p.Categoria!.Activa = false;
        Assert.False(p.EstaDisponible(Ahora));
    }

    [Fact]
    public void Sin_variantes_activas_no_esta_disponible()
    {
        var p = Producto();
        p.Variantes[0].Activa = false;
        Assert.False(p.EstaDisponible(Ahora));
    }

    [Theory]
    [InlineData(0, 1, true)]
    [InlineData(1, 2, false)]
    [InlineData(-2, 0, false)]
    public void Respeta_las_fechas_de_venta(int desde, int hasta, bool esperado)
    {
        var p = Producto();
        p.VentaDesde = Ahora.AddDays(desde);
        p.VentaHasta = Ahora.AddDays(hasta);
        Assert.Equal(esperado, p.EstaDisponible(Ahora));
    }

    [Theory]
    [InlineData(39999.99, 5000)]
    [InlineData(40000, 0)]
    public void El_envio_se_bonifica_desde_el_monto(double subtotal, double esperado)
    {
        var m = new MetodoEntrega { Nombre = "Envío", Tipo = TipoEntrega.Envio, Costo = 5000, GratisDesde = 40000 };
        Assert.Equal((decimal)esperado, m.CalcularCosto((decimal)subtotal));
    }

    [Fact]
    public void Sin_monto_de_bonificacion_siempre_se_cobra() =>
        Assert.Equal(5000, new MetodoEntrega { Nombre = "Envío", Tipo = TipoEntrega.Envio, Costo = 5000 }.CalcularCosto(999999));

    [Fact]
    public void El_retiro_no_se_cobra() =>
        Assert.Equal(0, new MetodoEntrega { Nombre = "Retiro", Tipo = TipoEntrega.Retiro, Costo = 5000 }.CalcularCosto(1));
}
