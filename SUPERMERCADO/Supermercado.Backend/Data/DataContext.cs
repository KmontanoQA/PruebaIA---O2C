using Microsoft.EntityFrameworkCore;
using Supermercado.Shared.Entities;

namespace Supermercado.Backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    // Entidades existentes
    public DbSet<Categoria_Producto> Categoria_Productos { get; set; }
    public DbSet<Rol> Rols { get; set; }

    // Nuevas entidades ERP O2C
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InventoryMove> InventoryMoves { get; set; }
    public DbSet<OrderAudit> OrderAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índices únicos existentes
        modelBuilder.Entity<Rol>().HasIndex(x => x.nombre).IsUnique();
        modelBuilder.Entity<Categoria_Producto>().HasIndex(x => x.descripcion).IsUnique();

        // Configuración de User
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // Configuración de Customer
        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.TaxId)
            .IsUnique();

        // Configuración de Product
        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Sku)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configuración de Order
        modelBuilder.Entity<Order>()
            .HasIndex(x => x.Number)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderLines)
            .WithOne(ol => ol.Order)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de OrderLine
        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.Product)
            .WithMany(p => p.OrderLines)
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de Invoice
        modelBuilder.Entity<Invoice>()
            .HasIndex(x => x.Number)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithOne(o => o.Invoice)
            .HasForeignKey<Invoice>(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de InventoryMove
        modelBuilder.Entity<InventoryMove>()
            .HasOne(im => im.Product)
            .WithMany(p => p.InventoryMoves)
            .HasForeignKey(im => im.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryMove>()
            .HasIndex(x => new { x.ProductId, x.CreatedAt });

        // Configuración de OrderAudit
        modelBuilder.Entity<OrderAudit>()
            .HasOne(oa => oa.Order)
            .WithMany()
            .HasForeignKey(oa => oa.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderAudit>()
            .HasOne(oa => oa.Seller)
            .WithMany()
            .HasForeignKey(oa => oa.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderAudit>()
            .HasIndex(x => new { x.OrderId, x.CreatedAt });

        // Configuración de Seller
        modelBuilder.Entity<Seller>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<Seller>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // Configuración de Order con Seller
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Seller)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
