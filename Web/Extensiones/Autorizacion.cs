using InventoryManagement.Web.Models.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Web.Authorizations
{
	public class ModuloAuthorizationRequirement : IAuthorizationRequirement
	{
		public string Modulo { get; private set; }

		public ModuloAuthorizationRequirement(string modulo)
		{
			Modulo = modulo;
		}
	}

	/// <summary>
	/// Autoriza cuando la empresa de la sesión tiene el módulo activo y el rol del usuario
	/// en esa empresa tiene asignada la pantalla (área + controlador) que se está abriendo.
	/// </summary>
	public class ModuloAuthorizationHandler : AuthorizationHandler<ModuloAuthorizationRequirement>
	{
		private readonly IHttpContextAccessor _httpContext;

		public ModuloAuthorizationHandler(IHttpContextAccessor httpContext)
		{
			_httpContext = httpContext;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModuloAuthorizationRequirement requirement)
		{
			var httpContext = _httpContext.HttpContext;
			var empresa = httpContext?.Session.GetObjectFromJson<EmpresaViewModel>("Empresa");
			if (httpContext != null && empresa != null && empresa.Modulos.Contains(requirement.Modulo))
			{
				var routeValues = httpContext.Request.RouteValues;
				var userId = context.User.GetUserId<int>();
				if (userId != 0)
				{
					var controlador = routeValues["controller"]?.ToString();
					var area = routeValues["area"]?.ToString();
					var hasPantalla = empresa.Pantallas.Any(m => m.Controlador == controlador && m.Area == area);
					if (hasPantalla)
					{
						context.Succeed(requirement);
						return Task.CompletedTask;
					}
				}
			}
			context.Fail();
			return Task.CompletedTask;
		}
	}
}
