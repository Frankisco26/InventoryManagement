using InventoryManagement.Models.Identidad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class LoginModel : PageModel
	{
		private readonly SignInManager<Usuario> _signInManager;
		private readonly UserManager<Usuario> _userManager;
		private readonly ILogger<LoginModel> _logger;

		public LoginModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<LoginModel> logger)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public string? ReturnUrl { get; set; }

		[TempData]
		public string? ErrorMessage { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Ingrese su usuario")]
			[Display(Name = "Usuario")]
			public string Email { get; set; } = string.Empty;

			[Required(ErrorMessage = "Ingrese su contraseña")]
			[DataType(DataType.Password)]
			[Display(Name = "Contraseña")]
			public string Password { get; set; } = string.Empty;

			[Display(Name = "Recordar en este equipo")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string? returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
				ModelState.AddModelError(string.Empty, ErrorMessage);

			// Limpiar la cookie externa para asegurar un ingreso limpio
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ReturnUrl = Url.Content("~/");
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl = Url.Content("~/");
			if (!ModelState.IsValid)
				return Page();

			var user = await _userManager.FindByNameAsync(Input.Email);

			// Igual que Contagestor: LockoutEnabled indica que el usuario fue bloqueado desde la administración
			if (user != null && ((user.LockoutEnd != null && user.LockoutEnd.Value > DateTimeOffset.Now) || user.LockoutEnabled))
			{
				_logger.LogWarning($"Usuario {user.UserName} bloqueado.");
				ModelState.AddModelError(string.Empty, $"El usuario {user.UserName} se encuentra bloqueado.");
				return Page();
			}

			var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				HttpContext.Session.Remove("Empresa");
				_logger.LogInformation("Usuario conectado.");
				return LocalRedirect(returnUrl);
			}
			if (result.IsLockedOut)
			{
				ModelState.AddModelError(string.Empty, "Se han realizado muchos intentos de ingreso");
				return Page();
			}

			ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
			return Page();
		}
	}
}
