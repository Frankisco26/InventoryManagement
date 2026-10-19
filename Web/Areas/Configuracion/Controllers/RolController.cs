using InventoryManagement.Data.Base;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Web.Areas.Configuracion.Models;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Areas.Configuracion.Controllers
{
	[Route("Configuracion/Rol")]
	[Area("Configuracion")]
	[Authorize(nameof(Modulos.Configuracion))]
	public class RolController : BaseMvcController
	{
		private readonly ContextoIdentidad _identidad;
		private readonly RoleManager<Rol> _roleManager;

		public RolController(ContextoPrincipal context, ContextoIdentidad identidad, RoleManager<Rol> roleManager, ILoggerFactory logger) : base(context, logger)
		{
			_identidad = identidad;
			_roleManager = roleManager;
		}

		[Route("Inicio")]
		public async Task<IActionResult> Inicio()
		{
			ViewBag.Empresas = await _context.Empresas.AsNoTracking()
				.OrderBy(m => m.Nombre)
				.Select(m => new { m.Nombre, m.Identificacion })
				.ToListAsync();

			return View();
		}

		[HttpPost]
		[Route("Listado")]
		public async Task<IActionResult> Listado()
		{
			try
			{
				var empresas = await _context.Empresas.AsNoTracking().Select(m => new { m.Nombre, m.Identificacion }).ToListAsync();
				var modelos = await _identidad.Roles.AsNoTracking().Select(rol => new RolViewModels
				{
					Id = rol.Id,
					Nombre = rol.Name ?? string.Empty,
					IdentificacionEmpresa = rol.Empresa,
					UsuarioId = rol.UsuarioCreacionId,
					IdentificacionEmpresaPadre = rol.IdentificacionEmpresaPadre,
					PermiteCrear = rol.PermiteCrear,
					PermiteEditar = rol.PermiteEditar,
					PermiteAnular = rol.PermiteAnular
				}).ToListAsync();

				foreach (var item in modelos)
					item.NombreEmpresa = empresas.FirstOrDefault(m => m.Identificacion == item.IdentificacionEmpresa)?.Nombre;

				return Json(modelos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new List<dynamic>());
			}
		}

		[Route("CrearRol")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CrearRol(RolViewModels modelo)
		{
			try
			{
				var empresa = await _context.Empresas.FirstOrDefaultAsync(s => s.Identificacion == modelo.IdentificacionEmpresa);
				if (empresa == null)
					throw new InvalidOperationException("La empresa no esta creada.");

				var nombre = (modelo.Nombre ?? string.Empty).Trim();
				if (await _identidad.Roles.AnyAsync(m => m.Empresa == modelo.IdentificacionEmpresa && m.NormalizedName == nombre.ToUpper()))
					return Json(new { tipo = "e", mensaje = "Ya existe el nombre de este rol para esta empresa", op = false });

				var registro = new Rol
				{
					Empresa = modelo.IdentificacionEmpresa,
					Name = nombre,
					NormalizedName = nombre.ToUpper(),
					UsuarioCreacionId = UsuarioId,
					FechaCreacion = DateTime.Now,
					IdentificacionEmpresaPadre = User.GetUserBusiness(),
					PermiteCrear = modelo.PermiteCrear,
					PermiteEditar = modelo.PermiteEditar,
					PermiteAnular = modelo.PermiteAnular
				};

				await AsignarPantallasAsync(registro);
				_identidad.Roles.Add(registro);
				await _identidad.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return Json(new { tipo = "e", mensaje = ex.Message, op = false });
			}

			return Json(new { tipo = "s", mensaje = "Se ha guardado correctamente", op = true });
		}

		[Route("Eliminar")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Eliminar(int id)
		{
			try
			{
				if (id == DatosIniciales.RolAdministradorId)
					return Json("No se puede eliminar el rol administrador de la empresa principal");

				if (await _identidad.UsuarioRoles.AnyAsync(ur => ur.RoleId == id))
					return Json("El rol tiene usuarios asignados; quíteselo a los usuarios antes de eliminarlo");

				var registro = await _roleManager.Roles.Include(t => t.Pantallas).FirstOrDefaultAsync(t => t.Id == id);
				if (registro == null)
					return Json("No se encontró el rol");

				await _roleManager.DeleteAsync(registro);
				return Json("correcto");
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}
		}

		[HttpPost]
		[Route("Editar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Editar(int id, RolViewModels modelo)
		{
			var nombre = (modelo.Nombre ?? string.Empty).Trim();
			if (await _identidad.Roles.AnyAsync(m => m.Empresa == modelo.IdentificacionEmpresa && m.NormalizedName == nombre.ToUpper() && m.Id != id))
				return Json(new { tipo = "e", mensaje = "Ya existe el nombre de este rol para esta empresa", op = false });
			try
			{
				var registro = await _roleManager.Roles.Include(t => t.Pantallas).FirstOrDefaultAsync(t => t.Id == id)
					?? throw new KeyNotFoundException("No se encontró el rol");

				registro.Name = nombre;
				registro.NormalizedName = nombre.ToUpper();
				registro.Empresa = modelo.IdentificacionEmpresa;
				registro.UsuarioModificacionId = UsuarioId;
				registro.FechaModificacion = DateTime.Now;
				registro.PermiteCrear = modelo.PermiteCrear;
				registro.PermiteEditar = modelo.PermiteEditar;
				registro.PermiteAnular = modelo.PermiteAnular;

				_identidad.RolPantallas.RemoveRange(registro.Pantallas);
				registro.Pantallas.Clear();
				await AsignarPantallasAsync(registro);

				await _identidad.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return Json(new { tipo = "e", mensaje = ex.Message, op = false });
			}

			return Json(new { tipo = "s", mensaje = "Se ha editado correctamente", op = true });
		}

		/// <summary>
		/// Agrega al rol las pantallas marcadas en el formulario (campos "Pantallas[n]_{area}" con el nombre de la pantalla).
		/// </summary>
		private async Task AsignarPantallasAsync(Rol registro)
		{
			var keyPantallas = Request.Form.Keys.Where(m => m.StartsWith("Pantallas")).ToList();
			foreach (var item in keyPantallas)
			{
				var nombrePantalla = Request.Form[item].FirstOrDefault();
				if (string.IsNullOrEmpty(nombrePantalla)) continue;
				var area = item.Split("_")[1];
				if (area == "null") area = null;
				var pantalla = await _identidad.Pantallas.FirstOrDefaultAsync(t => t.Name == nombrePantalla && t.Area == area);
				if (pantalla != null && !registro.Pantallas.Any(p => p.PantallaId == pantalla.Id))
					registro.Pantallas.Add(new RolPantalla { Rol = registro, Pantalla = pantalla, PantallaId = pantalla.Id });
			}
		}

		#region Ajax

		[Route("ObtenerRol")]
		[HttpPost]
		public async Task<JsonResult> ObtenerRol(int? id, string? empresaId)
		{
			try
			{
				var ruc = string.IsNullOrWhiteSpace(empresaId) ? User.GetUserBusiness() : empresaId;
				Rol? registro = null;
				if (id != null && id != 0)
				{
					registro = await _identidad.Roles.Include(t => t.Pantallas).ThenInclude(s => s.Pantalla).FirstOrDefaultAsync(t => t.Id == id.Value);
					if (registro != null && string.IsNullOrWhiteSpace(empresaId))
						ruc = registro.Empresa;
				}

				var modulos = await _context.Modulos.Where(t => t.Empresa!.Identificacion == ruc && t.Activo).Select(m => m.Codigo).Distinct().ToListAsync();
				var nombresModulos = Modulos.ObtenerNombresModulos();

				var modelosPantallas = await _identidad.Pantallas.Where(item => modulos.Contains(item.Module)).Select(s => new RolPantallaViewModel
				{
					Activa = false,
					Area = s.Area,
					Controlador = s.Controller,
					Modulo = s.Module,
					Nombre = s.Name,
					Orden = s.Order,
				}).ToListAsync();

				if (registro != null)
				{
					foreach (var item in modelosPantallas)
						item.Activa = registro.Pantallas.Any(m => m.Pantalla != null && m.Pantalla.Area == item.Area && m.Pantalla.Controller == item.Controlador && m.Pantalla.Name == item.Nombre);
				}

				var pantallas = modelosPantallas
					.OrderBy(t => t.Modulo).ThenBy(t => t.Orden)
					.GroupBy(t => t.Modulo)
					.Select(g => new
					{
						modulo = g.Key,
						nombreModulo = nombresModulos.TryGetValue(g.Key, out var n) ? n : g.Key,
						pantallas = g.ToList()
					});

				return Json(new
				{
					Id = registro?.Id ?? 0,
					Nombre = registro?.Name ?? string.Empty,
					RucEmpresa = registro?.Empresa ?? ruc,
					PermiteCrear = registro?.PermiteCrear ?? true,
					PermiteEditar = registro?.PermiteEditar ?? true,
					PermiteAnular = registro?.PermiteAnular ?? true,
					Pantallas = pantallas
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json("error");
			}
		}

		#endregion Ajax
	}
}
