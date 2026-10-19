using InventoryManagement.Data.Base;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Models.Utilitarios;
using InventoryManagement.Web.Areas.Configuracion.Models;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Areas.Configuracion.Controllers
{
	[Area("Configuracion")]
	[Route("Configuracion/Usuarios")]
	[Authorize(nameof(Modulos.Configuracion))]
	public class UsuariosController : BaseMvcController
	{
		private readonly ContextoIdentidad _identidad;
		private readonly UserManager<Usuario> _userManager;

		public UsuariosController(ContextoPrincipal context, ContextoIdentidad identidad, UserManager<Usuario> userManager, ILoggerFactory logger) : base(context, logger)
		{
			_identidad = identidad;
			_userManager = userManager;
		}

		[Route("Inicio")]
		public async Task<IActionResult> Inicio()
		{
			var roles = await _identidad.Roles.AsNoTracking().Select(s => new { s.Empresa, s.Name, s.Id }).ToListAsync();
			var empresas = await _context.Empresas.AsNoTracking().Where(s => s.Estatus == Estatus.ACTIVO).Select(s => new { s.Identificacion, s.Nombre }).ToListAsync();

			ViewBag.Roles = (from item in roles
							 join emp in empresas on item.Empresa equals emp.Identificacion
							 orderby emp.Nombre, item.Name
							 select new
							 {
								 Value = item.Id,
								 Name = $"{emp.Nombre} - {item.Name}"
							 }).ToList();
			return View();
		}

		[HttpPost]
		[Route("Listado")]
		public async Task<IActionResult> Listado()
		{
			try
			{
				var usuarios = await _userManager.Users.AsNoTracking().Select(s => new
				{
					BloqueoHabilitado = s.LockoutEnabled,
					Cuenta = s.UserName,
					FechaFin = s.LockoutEnd,
					s.Id,
					s.Name,
					s.Email
				}).ToListAsync();

				var claims = await _identidad.UsuarioReclamos.AsNoTracking()
					.Where(s => s.ClaimType == CustomClaimTypes.RucEmpresa)
					.Select(s => new { s.UserId, s.ClaimValue })
					.ToListAsync();
				var claimsEmpresas = claims.Select(s => s.ClaimValue).Distinct().ToList();
				var empresas = await _context.Empresas.AsNoTracking()
					.Where(s => claimsEmpresas.Contains(s.Identificacion))
					.Select(s => new { s.Identificacion, s.Nombre })
					.ToListAsync();

				var modelos = from item in usuarios
							  select new UsuarioIndexViewModel
							  {
								  Id = item.Id,
								  BloqueoHabilitado = item.BloqueoHabilitado,
								  Cuenta = item.Cuenta,
								  Email = item.Email,
								  FechaFin = item.FechaFin,
								  NombreEmpresa = string.Join(", ",
									from ce in claims
									join emp in empresas on ce.ClaimValue equals emp.Identificacion
									where ce.UserId == item.Id
									select emp.Identificacion + " - " + emp.Nombre),
								  Nombre = item.Name
							  };

				return Json(modelos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new List<dynamic>());
			}
		}

		[HttpPost]
		[Route("ObtenerUsuario")]
		public async Task<IActionResult> ObtenerUsuario(string id)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
					return Json(new { tipo = "e", msg = "No se pudo obtener el usuario." });

				var roles = await _identidad.UsuarioRoles
					.Where(ur => ur.UserId == user.Id)
					.Select(ur => ur.RoleId.ToString())
					.ToArrayAsync();

				var usuario = new UsuarioIndexViewModel
				{
					Id = user.Id,
					BloqueoHabilitado = user.LockoutEnabled,
					Cuenta = user.UserName,
					Email = user.Email,
					FechaFin = user.LockoutEnd,
					IdentificacionEmpresa = user.Empresa,
					Nombre = user.Name,
					Roles = roles
				};

				return Json(new { usuario });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", msg = "No se pudo obtener el usuario." });
			}
		}

		[HttpPost]
		[Route("ActivarUsuario")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ActivarUsuario(string id)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("No se encontró el usuario que se quiere activar.");
				user.LockoutEnd = null;
				user.LockoutEnabled = false;
				user.AccessFailedCount = 0;
				await _userManager.UpdateAsync(user);
				return Json(new { tipo = "s", msg = "El usuario fue activado correctamente." });
			}
			catch (Exception ex)
			{
				return Json(new { tipo = "e", msg = ex.Message });
			}
		}

		[HttpPost]
		[Route("BloquearUsuario")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BloquearUsuario(string id, DateTime? fecha)
		{
			try
			{
				if (fecha == null) throw new ArgumentException("No se ha especificado una fecha.");
				var user = await _userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("No se encontró el usuario que se quiere bloquear.");
				if (user.Id == UsuarioId) throw new InvalidOperationException("No puede bloquear su propio usuario.");

				user.LockoutEnd = fecha.Value.Date.Add(DateTime.Now.TimeOfDay);
				user.LockoutEnabled = true;
				await _userManager.UpdateAsync(user);
				await _userManager.UpdateSecurityStampAsync(user);
				return Json(new { tipo = "s", msg = "El usuario fue bloqueado correctamente." });
			}
			catch (Exception ex)
			{
				return Json(new { tipo = "e", msg = ex.Message });
			}
		}

		[HttpPost]
		[Route("RestablecerClave")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RestablecerClave(string id)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("No se encontró el usuario");
				var contrasena = GenerarContrasena();
				await CambiarContrasenaAsync(user, contrasena);
				return Json(new { tipo = "s", msg = $"Nueva contraseña de {user.UserName}: {contrasena}" });
			}
			catch (Exception ex)
			{
				return Json(new { tipo = "e", msg = ex.Message });
			}
		}

		/// <summary>
		/// Crea o actualiza el usuario. Los roles elegidos definen las empresas a las que tiene acceso
		/// (un claim RucEmpresa por cada empresa de sus roles), igual que en Contagestor.
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("CrearActualizar")]
		public async Task<IActionResult> CrearActualizar(UsuarioIndexViewModel usuario)
		{
			using var transaccion = await _identidad.Database.BeginTransactionAsync();
			try
			{
				var rolesIds = (usuario.Roles ?? Array.Empty<string>()).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
				var roles = await _identidad.Roles.Where(s => rolesIds.Contains(s.Id.ToString())).ToListAsync();
				if (!roles.Any())
					throw new ArgumentException("Debe asignar al menos un rol (empresa) al usuario.");

				var nombre = (usuario.Nombre ?? string.Empty).Trim();
				var cuenta = (usuario.Cuenta ?? string.Empty).Trim();
				var correo = (usuario.Email ?? string.Empty).Trim();
				if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.");
				if (string.IsNullOrWhiteSpace(cuenta)) throw new ArgumentException("El nombre de usuario es obligatorio.");

				var cuentaNormalizada = cuenta.ToUpperInvariant();
				if (await _identidad.Usuarios.AnyAsync(m => m.NormalizedUserName == cuentaNormalizada && m.Id != usuario.Id))
					throw new ArgumentException("El nombre de usuario ya se encuentra en uso.");

				var empresa = roles.First().Empresa;
				string? contrasenaGenerada = null;
				Usuario user;

				if (usuario.Id == 0)
				{
					user = new Usuario
					{
						Name = nombre,
						UserName = cuenta,
						Email = string.IsNullOrWhiteSpace(correo) ? null : correo,
						EmailConfirmed = true,
						LockoutEnabled = false,
						UsuarioCreacionId = UsuarioId,
						FechaCreacion = DateTime.Now,
						Empresa = empresa,
						IdentificacionEmpresaPadre = User.GetUserBusiness()
					};

					var contrasena = string.IsNullOrWhiteSpace(usuario.Clave) ? (contrasenaGenerada = GenerarContrasena()) : usuario.Clave;
					var creacion = await _userManager.CreateAsync(user, contrasena);
					if (!creacion.Succeeded)
						throw new InvalidOperationException(string.Join(" ", creacion.Errors.Select(e => e.Description)));
				}
				else
				{
					user = await _userManager.FindByIdAsync(usuario.Id.ToString()) ?? throw new KeyNotFoundException("No se encontró el usuario");
					user.Name = nombre;
					user.UserName = cuenta;
					user.Email = string.IsNullOrWhiteSpace(correo) ? null : correo;
					user.EmailConfirmed = true;
					user.UsuarioModificacionId = UsuarioId;
					user.FechaModificacion = DateTime.Now;
					user.Empresa = empresa;
					var actualizacion = await _userManager.UpdateAsync(user);
					if (!actualizacion.Succeeded)
						throw new InvalidOperationException(string.Join(" ", actualizacion.Errors.Select(e => e.Description)));

					if (!string.IsNullOrWhiteSpace(usuario.Clave))
						await CambiarContrasenaAsync(user, usuario.Clave);
				}

				// Roles
				var rolesUsuario = _identidad.UsuarioRoles.Where(m => m.UserId == user.Id);
				_identidad.UsuarioRoles.RemoveRange(rolesUsuario);
				await _identidad.SaveChangesAsync();
				foreach (var rol in roles)
					_identidad.UsuarioRoles.Add(new UsuarioRol { RoleId = rol.Id, UserId = user.Id });
				await _identidad.SaveChangesAsync();

				// Claims: nombre y empresas a las que tiene acceso
				var claimsActuales = await _userManager.GetClaimsAsync(user);
				foreach (var claim in claimsActuales.Where(s => s.Type == ClaimTypes.GivenName || s.Type == CustomClaimTypes.RucEmpresa))
					await _userManager.RemoveClaimAsync(user, claim);

				var claimsNuevos = new List<Claim> { new(ClaimTypes.GivenName, nombre) };
				claimsNuevos.AddRange(roles.Select(r => r.Empresa.Trim()).Distinct().Select(ruc => new Claim(CustomClaimTypes.RucEmpresa, ruc)));
				await _userManager.AddClaimsAsync(user, claimsNuevos);

				await transaccion.CommitAsync();

				var msg = contrasenaGenerada == null
					? "El usuario se ha guardado correctamente."
					: $"El usuario se ha guardado correctamente. Contraseña: {contrasenaGenerada}";
				return Json(new { tipo = "s", msg });
			}
			catch (Exception ex)
			{
				await transaccion.RollbackAsync();
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", msg = ex.Message });
			}
		}

		private async Task CambiarContrasenaAsync(Usuario user, string contrasena)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var resultado = await _userManager.ResetPasswordAsync(user, token, contrasena);
			if (!resultado.Succeeded)
				throw new InvalidOperationException($"No se pudo actualizar la contraseña: {string.Join("; ", resultado.Errors.Select(e => e.Description))}");
		}

		private static string GenerarContrasena() => RandomNumberGenerator.GetInt32(1000, 10000).ToString();
	}
}
