namespace System.Security.Claims
{
	public static class CustomClaimTypes
	{
		/// <summary>
		/// Identificación (RUC) de cada empresa a la que el usuario tiene acceso
		/// </summary>
		public const string RucEmpresa = "http://schemas.xmlsoap.org/ws/2019/05/identity/claims/businessidentifier";

		/// <summary>
		/// Última empresa elegida por el usuario cuando tiene acceso a más de una
		/// </summary>
		public const string EmpresaPredeterminada = "http://schemas.xmlsoap.org/ws/2019/05/identity/claims/defaultbusiness";
	}
}
