using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagement.Web.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class LockoutModel : PageModel
	{
		public string Titulo { get; private set; } = "Bloqueado.";
		public string Mensaje { get; private set; } = "Esta cuenta se encuentra inactiva.";

		public void OnGet(string? motivo = null)
		{
			if (motivo == "empresa")
			{
				Titulo = "Bloqueado";
				Mensaje = "Su acceso fue bloqueado. Por favor comuniquese con su administrador.";
			}
		}
	}
}
