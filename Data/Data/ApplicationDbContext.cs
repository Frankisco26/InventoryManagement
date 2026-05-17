using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de Product
            builder.Entity<Product>()
                .HasKey(p => p.Id);

            builder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            builder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Product>()
                .Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<Product>()
                .Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relación: Product -> StockMovements
            builder.Entity<Product>()
                .HasMany(p => p.StockMovements)
                .WithOne(sm => sm.Product)
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de StockMovement
            builder.Entity<StockMovement>()
                .HasKey(sm => sm.Id);

            builder.Entity<StockMovement>()
                .Property(sm => sm.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Entity<StockMovement>()
                .Property(sm => sm.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<StockMovement>()
                .Property(sm => sm.Type)
                .HasConversion<int>();
        }
    }

    /// <summary>
    /// Usuario de la aplicación extendido de IdentityUser
    /// </summary>
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
