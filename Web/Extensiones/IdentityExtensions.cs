using InventoryManagement.Web.Models.Session;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Identity
{
	public static class Extensions
	{
		public const string MensajeSesionCaducada = "Se ha caducado la sesión, por favor inicie sesión nuevamente.";

		private static IHttpContextAccessor? _contextAccessor;

		public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
		{
			_contextAccessor = httpContextAccessor;
		}

		public static T GetUserId<T>(this ClaimsPrincipal principal) where T : struct
		{
			var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			if (value != null)
				return (T)Convert.ChangeType(value, typeof(T));
			return default;
		}

		public static string? GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier);

		public static string GetUserFullName(this ClaimsPrincipal principal) => $"{principal.FindFirstValue(ClaimTypes.GivenName)}";

		#region Custom Claims

		public static IEnumerable<Claim> GetUserAllBusiness(this ClaimsPrincipal principal)
		{
			return principal.FindAll(CustomClaimTypes.RucEmpresa);
		}

		/// <summary>
		/// Empresa con la que el usuario está trabajando (guardada en la sesión).
		/// </summary>
		public static EmpresaViewModel GetEmpresaSesion(this ClaimsPrincipal principal)
		{
			var empresa = _contextAccessor?.HttpContext?.Session.GetObjectFromJson<EmpresaViewModel>("Empresa");
			if (empresa == null)
				throw new InvalidOperationException(MensajeSesionCaducada);
			return empresa;
		}

		/// <summary>
		/// Identificación (RUC) de la empresa con la que el usuario está trabajando.
		/// </summary>
		public static string GetUserBusiness(this ClaimsPrincipal principal) => principal.GetEmpresaSesion().Identificacion;

		#endregion Custom Claims
	}
}
