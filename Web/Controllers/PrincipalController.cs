using InventoryManagement.Models.Utilitarios;
using InventoryManagement.Web.Models;
using InventoryManagement.Web.Models.Session;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace InventoryManagement.Web.Controllers
{
	[Authorize]
	public class PrincipalController : Controller
	{
		private readonly EmpresasUsuario _empresasUsuario;

		public PrincipalController(EmpresasUsuario empresasUsuario)
		{
			_empresasUsuario = empresasUsuario;
		}

		public IActionResult CambiarEmpresa()
		{
			HttpContext.Session.Remove("Empresa");
			return View("Inicio");
		}

		/// <summary>
		/// Ruta por defecto: carga la empresa del usuario en la sesión y lo envía a su primera pantalla.
		/// Con <paramref name="empresa"/> cambia a esa empresa (selector de empresas).
		/// </summary>
		public async Task<IActionResult> Inicio(string empresa = "")
		{
			var model = HttpContext.Session.GetObjectFromJson<EmpresaViewModel>("Empresa");

			if (model == null || !string.IsNullOrWhiteSpace(empresa) || !User.HasClaim(m => m.Type == CustomClaimTypes.RucEmpresa))
			{
				var (empresaActual, estatus) = await _empresasUsuario.CargarEmpresaAsync(User.GetUserId<int>(), empresa);

				if (empresaActual == null)
				{
					HttpContext.Session.Remove("Empresa");
					if (estatus is Estatus.SUSPENDIDO or Estatus.INACTIVO)
						return RedirectToPage("/Account/Lockout", new { area = "Identity", motivo = "empresa" });

					return View();
				}

				model = empresaActual;
				HttpContext.Session.SetObjectAsJson("Empresa", model);
			}

			ViewBag.NombreEmpresa = model.Nombre;
			ViewBag.IdentificacionEmpresa = model.Identificacion;

			// Panel inicial (Configuracion/Resumen), igual que en Contagestor
			return RedirectToAction("Inicio", "Principal", new { Area = "Configuracion" });
		}

		[AllowAnonymous]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
