using InventoryManagement.Data.Base;
using InventoryManagement.Models.Administracion;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Models.Utilitarios;
using InventoryManagement.Web.Areas.Configuracion.Models;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Areas.Configuracion.Controllers
{
	[Area("Configuracion")]
	[Route("Configuracion/Empresas")]
	[Authorize(nameof(Modulos.Configuracion))]
	public class EmpresasController : BaseMvcController
	{
		private readonly ContextoIdentidad _identidad;
		private readonly UserManager<Usuario> _userManager;

		public EmpresasController(ContextoPrincipal context, ContextoIdentidad identidad, UserManager<Usuario> userManager, ILoggerFactory logger) : base(context, logger)
		{
			_identidad = identidad;
			_userManager = userManager;
		}

		[Route("Inicio")]
		public IActionResult Inicio()
		{
			return View();
		}

		[HttpPost]
		[Route("Listado")]
		public async Task<IActionResult> Listado()
		{
			try
			{
				var nombresModulos = Modulos.ObtenerNombresModulos();
				var empresas = await _context.Empresas.AsNoTracking()
					.OrderBy(m => m.Estatus).ThenBy(m => m.Nombre)
					.Select(m => new
					{
						m.Id,
						m.Codigo,
						m.Identificacion,
						m.Nombre,
						m.Telefono,
						m.Correo,
						m.Estatus,
						Modulos = m.Modulos.Where(t => t.Activo).Select(t => t.Codigo).ToList()
					})
					.ToListAsync();

				var usuariosPorEmpresa = await _identidad.UsuarioReclamos
					.Where(c => c.ClaimType == CustomClaimTypes.RucEmpresa)
					.GroupBy(c => c.ClaimValue)
					.Select(g => new { Empresa = g.Key, Total = g.Count() })
					.ToDictionaryAsync(k => k.Empresa ?? string.Empty, v => v.Total);

				var data = empresas.Select(item => new EmpresaIndexViewModel
				{
					Id = item.Id,
					Codigo = item.Codigo,
					Identificacion = item.Identificacion,
					Nombre = item.Nombre,
					Telefono = item.Telefono,
					Correo = item.Correo,
					Estatus = item.Estatus.ToString(),
					Usuarios = usuariosPorEmpresa.TryGetValue(item.Identificacion, out var total) ? total : 0,
					Modulos = string.Join(", ", item.Modulos.Select(c => nombresModulos.TryGetValue(c, out var n) ? n : c))
				});

				return Json(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new List<dynamic>());
			}
		}

		[Route("Crear")]
		public async Task<IActionResult> Crear(int id)
		{
			var nombresModulos = Modulos.ObtenerNombresModulos();

			if (id == 0)
			{
				var nuevo = new EmpresaEditViewModel
				{
					Modulos = nombresModulos.Select(m => new ModuloViewModel { Codigo = m.Key, Nombre = m.Value, Activo = true }).ToList()
				};
				return View(nuevo);
			}

			var empresa = await _context.Empresas.AsNoTracking().Include(t => t.Modulos).FirstOrDefaultAsync(t => t.Id == id);
			if (empresa == null)
			{
				TempData["tipo"] = "e";
				TempData["mensaje"] = "No se encontró la empresa";
				return RedirectToAction(nameof(Inicio));
			}

			var modulosEmpresa = empresa.Modulos.Where(t => t.Activo).Select(t => t.Codigo).ToList();
			var model = new EmpresaEditViewModel
			{
				Id = empresa.Id,
				Codigo = empresa.Codigo,
				Identificacion = empresa.Identificacion,
				Nombre = empresa.Nombre,
				RazonSocial = empresa.RazonSocial,
				ActividadEcononica = empresa.ActividadEcononica,
				Tipo = empresa.Tipo,
				Estatus = empresa.Estatus,
				IdentificacionRepresentanteLegal = empresa.IdentificacionRepresentanteLegal,
				NombreRepresentanteLegal = empresa.NombreRepresentanteLegal,
				LlevaContable = empresa.LlevaContable,
				Telefono = empresa.Telefono,
				Direccion = empresa.Direccion,
				Correo = empresa.Correo ?? string.Empty,
				PersonaContacto = empresa.PersonaContacto,
				Modulos = nombresModulos.Select(m => new ModuloViewModel { Codigo = m.Key, Nombre = m.Value, Activo = modulosEmpresa.Contains(m.Key) }).ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Crea o actualiza la empresa con sus módulos. Al crearla se generan el rol Administrador
		/// (con las pantallas de sus módulos) y el usuario administrador, igual que en Contagestor:
		/// usuario = correo de la empresa, contraseña = primeros 10 dígitos del RUC.
		/// </summary>
		[HttpPost]
		[Route("CrearActualizar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CrearActualizar(EmpresaEditViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Distinct();
				return Json(new { tipo = "e", mensaje = string.Join(" ", errores), op = false });
			}

			var identificacion = model.Identificacion.Trim();
			var correo = model.Correo.Trim();

			using var transaccionPrincipal = await _context.Database.BeginTransactionAsync();
			using var transaccionIdentidad = await _identidad.Database.BeginTransactionAsync();
			try
			{
				if (await _context.Empresas.AnyAsync(m => m.Identificacion == identificacion && m.Id != model.Id))
					throw new InvalidOperationException($"La identificación {identificacion} se encuentra en uso");

				if (await _context.Empresas.AnyAsync(m => m.Correo == correo && m.Id != model.Id))
					throw new InvalidOperationException($"El correo {correo} se encuentra en uso");

				var agregarEmpresa = model.Id == 0;
				Empresa registro;
				if (agregarEmpresa)
				{
					registro = new Empresa
					{
						UsuarioCreacionId = UsuarioId,
						FechaCreacion = DateTime.Now,
						Codigo = $"{await _context.Empresas.CountAsync() + 1:0000000}",
						IdentificacionEmpresaPadre = User.GetUserBusiness()
					};
					_context.Empresas.Add(registro);
				}
				else
				{
					registro = await _context.Empresas.Include(s => s.Modulos).FirstOrDefaultAsync(s => s.Id == model.Id)
						?? throw new KeyNotFoundException("No se encontró la empresa");

					if (registro.Identificacion != identificacion)
						throw new InvalidOperationException("No se puede cambiar el RUC de una empresa existente");

					registro.UsuarioModificacionId = UsuarioId;
					registro.FechaModificacion = DateTime.Now;
				}

				registro.Principal = true;
				registro.Identificacion = identificacion;
				registro.Nombre = model.Nombre.Trim();
				registro.RazonSocial = model.RazonSocial.Trim();
				registro.ActividadEcononica = model.ActividadEcononica;
				registro.Tipo = model.Tipo;
				registro.Estatus = model.Estatus;
				registro.IdentificacionRepresentanteLegal = model.IdentificacionRepresentanteLegal;
				registro.NombreRepresentanteLegal = model.NombreRepresentanteLegal;
				registro.LlevaContable = model.LlevaContable;
				registro.Telefono = model.Telefono;
				registro.Direccion = model.Direccion;
				registro.Correo = correo;
				registro.PersonaContacto = model.PersonaContacto;

				#region Módulos

				var modulosElegidos = Request.Form.Keys.Where(t => t.StartsWith("M0")).ToList();
				if (!modulosElegidos.Any())
					modulosElegidos = new List<string> { Modulos.Configuracion, Modulos.Stock };

				foreach (var modulo in registro.Modulos)
				{
					modulo.Activo = modulosElegidos.Contains(modulo.Codigo);
					modulo.UsuarioModificacionId = UsuarioId;
					modulo.FechaModificacion = DateTime.Now;
				}

				foreach (var codigo in modulosElegidos.Where(c => !registro.Modulos.Any(m => m.Codigo == c)))
				{
					registro.Modulos.Add(new Modulo(codigo)
					{
						UsuarioCreacionId = UsuarioId,
						FechaCreacion = DateTime.Now
					});
				}

				await _context.SaveChangesAsync();

				#endregion Módulos

				#region Rol Administrador y acceso a pantallas

				var rol = await _identidad.Roles.Include(s => s.Pantallas)
					.FirstOrDefaultAsync(s => s.Name == DatosIniciales.RolAdministrador && s.Empresa == registro.Identificacion);
				if (rol == null)
				{
					rol = new Rol
					{
						Name = DatosIniciales.RolAdministrador,
						NormalizedName = DatosIniciales.RolAdministrador.ToUpperInvariant(),
						Empresa = registro.Identificacion,
						IdentificacionEmpresaPadre = User.GetUserBusiness(),
						UsuarioCreacionId = UsuarioId,
						FechaCreacion = DateTime.Now
					};
					_identidad.Roles.Add(rol);
				}

				var modulosActivos = registro.Modulos.Where(t => t.Activo).Select(t => t.Codigo).ToList();
				var pantallas = await _identidad.Pantallas.Where(t => modulosActivos.Contains(t.Module)).ToListAsync();

				// La administración de empresas, usuarios y permisos queda solo para la empresa principal
				if (registro.Identificacion != DatosIniciales.EmpresaPrincipalIdentificacion)
					pantallas = pantallas.Where(t => !DatosIniciales.PantallasExcluidas.Contains(t.Name)).ToList();

				foreach (var pantalla in pantallas.Where(p => !rol.Pantallas.Any(s => s.PantallaId == p.Id)))
					rol.Pantallas.Add(new RolPantalla { Rol = rol, Pantalla = pantalla });

				await _identidad.SaveChangesAsync();

				#endregion Rol Administrador y acceso a pantallas

				var mensaje = "Se ha guardado correctamente";

				#region Usuario administrador

				if (agregarEmpresa)
				{
					var usuario = await _userManager.FindByNameAsync(correo);
					string? contrasena = null;
					if (usuario == null)
					{
						contrasena = identificacion.Length > 10 ? identificacion[..10] : identificacion;
						usuario = new Usuario
						{
							Name = string.IsNullOrWhiteSpace(model.NombreRepresentanteLegal) ? registro.Nombre : model.NombreRepresentanteLegal.Trim(),
							UserName = correo,
							Email = correo,
							EmailConfirmed = true,
							PhoneNumber = model.Telefono,
							LockoutEnabled = false,
							Empresa = registro.Identificacion,
							IdentificacionEmpresaPadre = User.GetUserBusiness(),
							UsuarioCreacionId = UsuarioId,
							FechaCreacion = DateTime.Now
						};
						var creacion = await _userManager.CreateAsync(usuario, contrasena);
						if (!creacion.Succeeded)
							throw new InvalidOperationException(string.Join(" ", creacion.Errors.Select(e => e.Description)));

						await _userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.GivenName, usuario.Name));
					}

					if (!await _identidad.UsuarioRoles.AnyAsync(m => m.UserId == usuario.Id && m.RoleId == rol.Id))
						_identidad.UsuarioRoles.Add(new UsuarioRol { UserId = usuario.Id, RoleId = rol.Id });

					if (!await _identidad.UsuarioReclamos.AnyAsync(m => m.UserId == usuario.Id && m.ClaimType == CustomClaimTypes.RucEmpresa && m.ClaimValue == registro.Identificacion))
						await _userManager.AddClaimAsync(usuario, new Claim(CustomClaimTypes.RucEmpresa, registro.Identificacion));

					await _identidad.SaveChangesAsync();

					mensaje = contrasena == null
						? $"Se ha guardado correctamente. El usuario {correo} ya existía y se le dio acceso a la empresa."
						: $"Se ha guardado correctamente. Usuario administrador: {correo} - Contraseña: {contrasena}";
				}

				#endregion Usuario administrador

				await transaccionPrincipal.CommitAsync();
				await transaccionIdentidad.CommitAsync();

				TempData["tipo"] = "s";
				TempData["mensaje"] = "Se ha guardado correctamente";
				return Json(new { tipo = "s", mensaje, op = true });
			}
			catch (Exception ex)
			{
				await transaccionPrincipal.RollbackAsync();
				await transaccionIdentidad.RollbackAsync();
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", mensaje = ex.Message, op = false });
			}
		}

		[HttpPost]
		[Route("CambiarEstado")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CambiarEstado(int id, Estatus estado)
		{
			try
			{
				var empresa = await _context.Empresas.FirstOrDefaultAsync(t => t.Id == id);
				if (empresa == null)
					return Json(new { tipo = "e", mensaje = "No se encontró la empresa" });

				if (empresa.Identificacion == DatosIniciales.EmpresaPrincipalIdentificacion && estado != Estatus.ACTIVO)
					return Json(new { tipo = "e", mensaje = "La empresa principal no se puede bloquear" });

				empresa.Estatus = estado;
				empresa.UsuarioModificacionId = UsuarioId;
				empresa.FechaModificacion = DateTime.Now;
				await _context.SaveChangesAsync();
				return Json(new { tipo = "c", mensaje = "Cambio realizado correctamente" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", mensaje = ex.Message });
			}
		}
	}
}
