using InventoryManagement.Models.Identidad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagement.Web.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	[IgnoreAntiforgeryToken]
	public class LogoutModel : PageModel
	{
		private readonly SignInManager<Usuario> _signInManager;
		private readonly ILogger<LogoutModel> _logger;

		public LogoutModel(SignInManager<Usuario> signInManager, ILogger<LogoutModel> logger)
		{
			_signInManager = signInManager;
			_logger = logger;
		}

		public async Task<IActionResult> OnGet()
		{
			await CerrarSesionAsync();
			return RedirectToPage("./Login");
		}

		public async Task<IActionResult> OnPost(string? returnUrl = null)
		{
			await CerrarSesionAsync();
			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			return RedirectToPage("./Login");
		}

		private async Task CerrarSesionAsync()
		{
			await _signInManager.SignOutAsync();
			HttpContext.Session.Clear();
			_logger.LogInformation("Usuario desconectado.");
		}
	}
}
