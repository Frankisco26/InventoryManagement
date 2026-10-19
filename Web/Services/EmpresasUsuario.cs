using InventoryManagement.Data.Base;
using InventoryManagement.Models.Administracion;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Models.Utilitarios;
using InventoryManagement.Web.Models.Session;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Services
{
	/// <summary>
	/// Resuelve a qué empresas tiene acceso un usuario y arma la información de la empresa activa
	/// (módulos y pantallas permitidas) que se guarda en la sesión.
	/// </summary>
	public class EmpresasUsuario
	{
		public static readonly Dictionary<string, string[]> NombresModulos = new()
		{
			{ Modulos.Configuracion, new[] { "Configuración", "users-cog" } },
			{ Modulos.Stock, new[] { "Stock", "box" } }
		};

		private readonly ContextoPrincipal _contexto;
		private readonly ContextoIdentidad _identidad;

		public EmpresasUsuario(ContextoPrincipal contexto, ContextoIdentidad identidad)
		{
			_contexto = contexto;
			_identidad = identidad;
		}

		/// <summary>
		/// Empresas (RUC, nombre) asignadas al usuario mediante sus claims de empresa.
		/// </summary>
		public async Task<IDictionary<string, string>> GetForUserAsync(int userId)
		{
			var ids = await _identidad.UsuarioReclamos
				.Where(m => m.UserId == userId && m.ClaimType == CustomClaimTypes.RucEmpresa)
				.Select(m => m.ClaimValue)
				.ToListAsync();

			return await _contexto.Empresas
				.Where(m => ids.Contains(m.Identificacion))
				.OrderBy(m => m.Nombre)
				.ToDictionaryAsync(k => k.Identificacion, v => v.Nombre);
		}

		/// <summary>
		/// Determina la empresa activa del usuario. Si no se indica una y el usuario tiene rol en varias,
		/// se usa la última que eligió (claim de empresa predeterminada).
		/// </summary>
		public async Task<(EmpresaViewModel? Empresa, Estatus? Estatus)> CargarEmpresaAsync(int usuarioId, string? identificacion = null)
		{
			List<Empresa> empresasPrincipales;
			if (string.IsNullOrWhiteSpace(identificacion))
				empresasPrincipales = await _contexto.Empresas.AsNoTracking().Where(e => e.Principal).ToListAsync();
			else
				empresasPrincipales = await _contexto.Empresas.AsNoTracking().Where(e => e.Identificacion == identificacion).ToListAsync();

			var empresasIdPrincipales = empresasPrincipales.Select(t => t.Identificacion).ToArray();
			var rolesIdAll = await _identidad.UsuarioRoles.Where(s => s.UserId == usuarioId).Select(s => s.RoleId).ToArrayAsync();
			var rolesEmpresa = await _identidad.Roles.AsNoTracking()
				.Where(s => rolesIdAll.Contains(s.Id) && empresasIdPrincipales.Contains(s.Empresa))
				.ToListAsync();
			var empresasConRol = empresasPrincipales.Where(t => rolesEmpresa.Any(a => a.Empresa == t.Identificacion)).ToList();

			Empresa? empresaActual;
			if (!string.IsNullOrWhiteSpace(identificacion) || empresasConRol.Count <= 1)
			{
				empresaActual = empresasConRol.FirstOrDefault();
			}
			else
			{
				var rucPredeterminado = await _identidad.UsuarioReclamos
					.Where(s => s.UserId == usuarioId && s.ClaimType == CustomClaimTypes.EmpresaPredeterminada)
					.Select(s => s.ClaimValue).FirstOrDefaultAsync();
				empresaActual = empresasConRol.FirstOrDefault(t => t.Identificacion == rucPredeterminado) ?? empresasConRol.FirstOrDefault();
			}

			if (empresaActual == null)
				return (null, null);

			if (empresaActual.Estatus != Estatus.ACTIVO)
				return (null, empresaActual.Estatus);

			if (empresasConRol.Count > 1)
				await GuardarEmpresaPredeterminadaAsync(usuarioId, empresaActual.Identificacion);

			var rolesEmpresaActual = rolesEmpresa.Where(a => a.Empresa == empresaActual.Identificacion).ToList();
			var rolesIdEmpresaActual = rolesEmpresaActual.Select(a => a.Id).ToArray();

			var model = new EmpresaViewModel
			{
				Id = empresaActual.Id,
				Identificacion = empresaActual.Identificacion,
				Nombre = empresaActual.Nombre,
				PermiteCrear = rolesEmpresaActual.Any(r => r.PermiteCrear),
				PermiteEditar = rolesEmpresaActual.Any(r => r.PermiteEditar),
				PermiteAnular = rolesEmpresaActual.Any(r => r.PermiteAnular)
			};

			model.Modulos = await _contexto.Modulos.AsNoTracking()
				.Where(a => a.EmpresaId == empresaActual.Id && a.Activo)
				.Select(m => m.Codigo)
				.ToListAsync();

			var pantallas = await _identidad.RolPantallas.AsNoTracking()
				.Where(s => rolesIdEmpresaActual.Contains(s.RolId))
				.Select(s => s.Pantalla!)
				.Distinct()
				.OrderBy(s => s.Order)
				.ToListAsync();

			foreach (var item in pantallas.Where(t => model.Modulos.Contains(t.Module)))
			{
				model.Pantallas.Add(new PantallaViewModel
				{
					Accion = item.Action,
					Area = item.Area,
					Controlador = item.Controller,
					Icon = NombresModulos.TryGetValue(item.Module, out var modulo) ? modulo[1] : "th-list",
					Modulo = item.Module,
					Nombre = item.Name,
					Orden = item.Order,
					Activo = item.Active
				});
			}

			return (model, empresaActual.Estatus);
		}

		private async Task GuardarEmpresaPredeterminadaAsync(int usuarioId, string identificacion)
		{
			var reclamo = await _identidad.UsuarioReclamos
				.FirstOrDefaultAsync(s => s.UserId == usuarioId && s.ClaimType == CustomClaimTypes.EmpresaPredeterminada);
			if (reclamo == null)
			{
				_identidad.UsuarioReclamos.Add(new UsuarioReclamo
				{
					UserId = usuarioId,
					ClaimType = CustomClaimTypes.EmpresaPredeterminada,
					ClaimValue = identificacion
				});
				await _identidad.SaveChangesAsync();
			}
			else if (reclamo.ClaimValue != identificacion)
			{
				reclamo.ClaimValue = identificacion;
				await _identidad.SaveChangesAsync();
			}
		}
	}
}
