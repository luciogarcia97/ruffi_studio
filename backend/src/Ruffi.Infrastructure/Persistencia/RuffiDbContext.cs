using Microsoft.EntityFrameworkCore;
using Ruffi.Domain.Catalogo;
using Ruffi.Domain.Entregas;

namespace Ruffi.Infrastructure.Persistencia;

public class RuffiDbContext(DbContextOptions<RuffiDbContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MetodoEntrega> MetodosEntrega => Set<MetodoEntrega>();

    protected override void ConfigureConventions(ModelConfigurationBuilder config)
    {
        config.Properties<decimal>().HavePrecision(12, 2);
        config.Properties<string>().HaveMaxLength(200);
        config.Properties<TipoOpcion>().HaveConversion<string>().HaveMaxLength(30);
        config.Properties<TipoEntrega>().HaveConversion<string>().HaveMaxLength(30);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Categoria>(e =>
        {
            e.ToTable("categorias");
            e.HasIndex(x => x.Slug).IsUnique();
        });
        model.Entity<Producto>(e =>
        {
            e.ToTable("productos");
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Descripcion).HasMaxLength(4000);
            e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Variantes).WithOne().HasForeignKey(x => x.ProductoId);
            e.HasMany(x => x.Imagenes).WithOne().HasForeignKey(x => x.ProductoId);
            e.HasMany(x => x.Opciones).WithOne().HasForeignKey(x => x.ProductoId);
        });
        model.Entity<Variante>().ToTable("variantes");
        model.Entity<Imagen>(e =>
        {
            e.ToTable("imagenes");
            e.Property(x => x.Url).HasMaxLength(500);
        });
        model.Entity<OpcionPersonalizacion>(e =>
        {
            e.ToTable("opciones_personalizacion");
            e.Property(x => x.Ayuda).HasMaxLength(500);
        });
        model.Entity<MetodoEntrega>().ToTable("metodos_entrega");
    }
}
