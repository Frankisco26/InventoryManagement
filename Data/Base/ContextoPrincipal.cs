using InventoryManagement.Models;
using InventoryManagement.Models.Administracion;
using InventoryManagement.Models.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Base
{
	public class ContextoPrincipal : DbContext
	{
		public ContextoPrincipal(DbContextOptions<ContextoPrincipal> options) : base(options)
		{
		}

		#region DbSets

		public DbSet<Empresa> Empresas { get; set; }
		public DbSet<Modulo> Modulos { get; set; }
		public DbSet<Parametro> Parametros { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<StockMovement> StockMovements { get; set; }

		#endregion DbSets

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Empresa>(ConfigureEmpresa);
			modelBuilder.Entity<Modulo>(ConfigureModulo);
			modelBuilder.Entity<Parametro>(ConfigureParametro);
			modelBuilder.Entity<Product>(ConfigureProduct);
			modelBuilder.Entity<StockMovement>(ConfigureStockMovement);

			Sembrar(modelBuilder);
		}

		private static void ConfigureEmpresa(EntityTypeBuilder<Empresa> e)
		{
			e.ToTable("Empresas", Entornos.Principal);
			e.HasKey(m => m.Id);

			e.Property(m => m.Id).ValueGeneratedOnAdd();
			e.Property(m => m.Codigo).HasMaxLength(Longitudes.Codigo).IsRequired();
			e.Property(m => m.Identificacion).HasMaxLength(Longitudes.Identificacion).IsRequired();
			e.Property(m => m.Nombre).HasMaxLength(Longitudes.Nombre).IsRequired();
			e.Property(m => m.RazonSocial).HasMaxLength(Longitudes.Nombre).IsRequired();
			e.Property(m => m.ActividadEcononica).HasMaxLength(Longitudes.Direccion);
			e.Property(m => m.Tipo).HasDefaultValue(Models.Utilitarios.Empresas.Desconocida).IsRequired();
			e.Property(m => m.IdentificacionRepresentanteLegal).HasMaxLength(Longitudes.Identificacion);
			e.Property(m => m.NombreRepresentanteLegal).HasMaxLength(Longitudes.Nombre);
			e.Property(m => m.Estatus).IsRequired();
			e.Property(m => m.IdentificacionEmpresaPadre).HasMaxLength(Longitudes.Identificacion);

			// Contactabilidad
			e.Property(m => m.Telefono).HasMaxLength(Longitudes.Nombre);
			e.Property(m => m.Direccion).HasMaxLength(Longitudes.Direccion);
			e.Property(m => m.Correo).HasMaxLength(Longitudes.Nombre);
			e.Property(m => m.PersonaContacto).HasMaxLength(Longitudes.Nombre);

			e.HasIndex(m => m.Identificacion).IsUnique();
		}

		private static void ConfigureModulo(EntityTypeBuilder<Modulo> e)
		{
			e.ToTable("Modulos", Entornos.Principal);
			e.HasKey(m => m.Id);

			e.HasOne(m => m.Empresa)
				.WithMany(m => m.Modulos).HasForeignKey(m => m.EmpresaId)
				.OnDelete(DeleteBehavior.Restrict);

			e.Property(m => m.Id).ValueGeneratedOnAdd();
			e.Property(m => m.Codigo).HasMaxLength(Longitudes.Codigo).IsRequired();
			e.Property(m => m.Activo).IsRequired();
		}

		private static void ConfigureParametro(EntityTypeBuilder<Parametro> e)
		{
			e.ToTable("Parametros", Entornos.Principal);
			e.HasKey(m => m.Id);

			e.HasOne(m => m.Empresa)
				.WithMany(m => m.Parametros).HasForeignKey(m => m.EmpresaId)
				.OnDelete(DeleteBehavior.Restrict);

			e.Property(m => m.Id).ValueGeneratedOnAdd();
			e.Property(m => m.Codigo).HasMaxLength(Longitudes.Codigo).IsRequired();
			e.Property(m => m.Nombre).HasMaxLength(Longitudes.Nombre).IsRequired();
			e.Property(m => m.Descripcion).HasMaxLength(Longitudes.Direccion);
		}

		private static void ConfigureProduct(EntityTypeBuilder<Product> e)
		{
			e.ToTable("Productos", Entornos.Principal);
			e.HasKey(p => p.Id);

			e.HasOne(p => p.Empresa)
				.WithMany(m => m.Productos).HasForeignKey(p => p.EmpresaId)
				.OnDelete(DeleteBehavior.Restrict);

			// El SKU es único dentro de cada empresa
			e.HasIndex(p => new { p.EmpresaId, p.SKU }).IsUnique();

			e.Property(p => p.Name).IsRequired().HasMaxLength(200);
			e.Property(p => p.SKU).IsRequired().HasMaxLength(50);
			e.Property(p => p.Category).IsRequired().HasMaxLength(100);
			e.Property(p => p.UnitPrice).HasPrecision(18, 2);
			e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

			e.HasMany(p => p.StockMovements)
				.WithOne(sm => sm.Product)
				.HasForeignKey(sm => sm.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private static void ConfigureStockMovement(EntityTypeBuilder<StockMovement> e)
		{
			e.ToTable("MovimientosStock", Entornos.Principal);
			e.HasKey(sm => sm.Id);

			e.Property(sm => sm.Reason).IsRequired().HasMaxLength(500);
			e.Property(sm => sm.Timestamp).HasDefaultValueSql("GETUTCDATE()");
			e.Property(sm => sm.Type).HasConversion<int>();
		}

		private static void Sembrar(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Empresa>().HasData(new Empresa
			{
				Id = DatosIniciales.EmpresaPrincipalId,
				Codigo = "0000001",
				Identificacion = DatosIniciales.EmpresaPrincipalIdentificacion,
				Nombre = DatosIniciales.EmpresaPrincipalNombre,
				RazonSocial = DatosIniciales.EmpresaPrincipalNombre,
				Tipo = Models.Utilitarios.Empresas.Comercial,
				Estatus = Estatus.ACTIVO,
				Principal = true
			});

			modelBuilder.Entity<Modulo>().HasData(
				new Modulo { Id = 1, Codigo = Areas.Configuracion, Activo = true, EmpresaId = DatosIniciales.EmpresaPrincipalId },
				new Modulo { Id = 2, Codigo = Areas.Stock, Activo = true, EmpresaId = DatosIniciales.EmpresaPrincipalId });
		}
	}
}
