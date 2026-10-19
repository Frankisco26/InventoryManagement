using InventoryManagement.Models.Identidad;
using InventoryManagement.Models.Utilitarios;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryManagement.Data.Base
{
	public class ContextoIdentidad : IdentityDbContext<Usuario, Rol, int, UsuarioReclamo, UsuarioRol, UsuarioAcceso, RolReclamo, UsuarioToken>
	{
		public ContextoIdentidad(DbContextOptions<ContextoIdentidad> options) : base(options)
		{
		}

		#region DbSets

		public DbSet<Usuario> Usuarios => Users;
		public DbSet<UsuarioReclamo> UsuarioReclamos => UserClaims;
		public DbSet<UsuarioAcceso> UsuarioAccesos => UserLogins;
		public DbSet<UsuarioToken> UsuarioTokens => UserTokens;
		public DbSet<UsuarioRol> UsuarioRoles => UserRoles;
		public DbSet<RolReclamo> RolReclamos => RoleClaims;
		public DbSet<RolPantalla> RolPantallas { get; set; }
		public DbSet<Pantalla> Pantallas { get; set; }

		#endregion DbSets

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Pantalla>(e =>
			{
				e.ToTable("Pantallas", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
				e.Property(m => m.Name).HasMaxLength(Longitudes.Nombre).IsRequired();
				e.Property(m => m.Module).HasMaxLength(Longitudes.Nombre).IsRequired();
				e.Property(m => m.Active).HasDefaultValue(true);
				e.Property(m => m.Area).HasMaxLength(Longitudes.Nombre);
				e.Property(m => m.Controller).HasMaxLength(Longitudes.Nombre);
				e.Property(m => m.Action).HasMaxLength(Longitudes.Nombre);

				e.HasIndex(m => m.Module);
				e.HasIndex(m => new { m.Area, m.Controller, m.Action });
				e.HasIndex(m => m.Order);
			});

			modelBuilder.Entity<Rol>(e =>
			{
				e.ToTable("Roles", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
				e.Property(m => m.Empresa).HasMaxLength(Longitudes.Identificacion).IsRequired();
				e.Property(m => m.IdentificacionEmpresaPadre).HasMaxLength(Longitudes.Identificacion);

				// El nombre del rol se repite entre empresas (cada empresa tiene su "Administrador")
				e.Metadata.RemoveIndex(new[] { e.Property(m => m.NormalizedName).Metadata });
				e.HasIndex(u => u.NormalizedName);
				e.HasIndex(m => m.Empresa);
				e.HasIndex(m => new { m.Name, m.Empresa }).IsUnique();
			});

			modelBuilder.Entity<RolReclamo>(e =>
			{
				e.ToTable("RolesReclamos", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
			});

			modelBuilder.Entity<RolPantalla>(e =>
			{
				e.ToTable("RolesPantallas", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
				e.HasOne(m => m.Rol).WithMany(m => m.Pantallas).HasForeignKey(m => m.RolId).OnDelete(DeleteBehavior.Cascade);
				e.HasOne(m => m.Pantalla).WithMany(m => m.Roles).HasForeignKey(m => m.PantallaId).OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Usuario>(e =>
			{
				e.ToTable("Usuarios", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
				e.Property(m => m.Name).HasMaxLength(Longitudes.Nombre).IsRequired();
				e.Property(m => m.Empresa).HasMaxLength(Longitudes.Identificacion);
				e.Property(m => m.IdentificacionEmpresaPadre).HasMaxLength(Longitudes.Identificacion);

				e.HasIndex(m => m.Empresa);
				e.HasIndex(m => new { m.Name, m.Empresa }).IsUnique();
			});

			modelBuilder.Entity<UsuarioRol>(e => e.ToTable("UsuariosRoles", Entornos.Identidad));

			modelBuilder.Entity<UsuarioReclamo>(e =>
			{
				e.ToTable("UsuariosReclamos", Entornos.Identidad);
				e.Property(m => m.Id).ValueGeneratedOnAdd();
			});

			modelBuilder.Entity<UsuarioAcceso>(e => e.ToTable("UsuariosAccesos", Entornos.Identidad));

			modelBuilder.Entity<UsuarioToken>(e => e.ToTable("UsuariosTokens", Entornos.Identidad));

			Sembrar(modelBuilder);
		}

		private static void Sembrar(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Pantalla>().HasData(DatosIniciales.Pantallas.Select(p => new Pantalla
			{
				Id = p.Id,
				Name = p.Nombre,
				Order = p.Orden,
				Module = p.Modulo,
				Area = p.Area,
				Controller = p.Controlador,
				Action = "Inicio",
				Active = true
			}));

			modelBuilder.Entity<Rol>().HasData(new Rol
			{
				Id = DatosIniciales.RolAdministradorId,
				Name = DatosIniciales.RolAdministrador,
				NormalizedName = DatosIniciales.RolAdministrador.ToUpperInvariant(),
				ConcurrencyStamp = "5d1e6c1a-6f43-4c1c-9a39-2c1f0f6a3b11",
				Empresa = DatosIniciales.EmpresaPrincipalIdentificacion,
				PermiteCrear = true,
				PermiteEditar = true,
				PermiteAnular = true
			});

			modelBuilder.Entity<RolPantalla>().HasData(DatosIniciales.Pantallas.Select(p => new RolPantalla
			{
				Id = p.Id,
				RolId = DatosIniciales.RolAdministradorId,
				PantallaId = p.Id
			}));

			// Usuario principal: mismo registro que en Contagestor (la contraseña es la misma)
			modelBuilder.Entity<Usuario>().HasData(new Usuario
			{
				Id = DatosIniciales.UsuarioPrincipalId,
				Name = "MANTENIMIENTO",
				UserName = "1727495366001",
				NormalizedUserName = "1727495366001",
				Email = "1727495366001",
				NormalizedEmail = "1727495366001",
				EmailConfirmed = true,
				PasswordHash = "AQAAAAIAAYagAAAAEHlHGRmIMuQlN/XPKn0GUK7WPf+5vtvh8ZWm+Q8lTtX4nQizBE44uFjL2q5+1WVIrQ==",
				SecurityStamp = "SP3KWAFXBUEQNJWYNX3ZXH5PNQ3FVKN5",
				ConcurrencyStamp = "48ab553d-012b-4098-844a-560d62b08bff",
				PhoneNumberConfirmed = false,
				TwoFactorEnabled = false,
				LockoutEnabled = false,
				AccessFailedCount = 0,
				Empresa = DatosIniciales.EmpresaPrincipalIdentificacion,
				FechaModificacion = new DateTime(2026, 10, 13, 17, 44, 40, 312, DateTimeKind.Unspecified).AddTicks(8231),
				UsuarioModificacionId = 1
			});

			modelBuilder.Entity<UsuarioRol>().HasData(new UsuarioRol
			{
				UserId = DatosIniciales.UsuarioPrincipalId,
				RoleId = DatosIniciales.RolAdministradorId
			});

			modelBuilder.Entity<UsuarioReclamo>().HasData(
				new UsuarioReclamo
				{
					Id = 1,
					UserId = DatosIniciales.UsuarioPrincipalId,
					ClaimType = ClaimTypes.GivenName,
					ClaimValue = "MANTENIMIENTO"
				},
				new UsuarioReclamo
				{
					Id = 2,
					UserId = DatosIniciales.UsuarioPrincipalId,
					ClaimType = CustomClaimTypes.RucEmpresa,
					ClaimValue = DatosIniciales.EmpresaPrincipalIdentificacion
				});
		}
	}
}
