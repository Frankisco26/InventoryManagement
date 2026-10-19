namespace InventoryManagement.Models.Utilitarios
{
	/// <summary>
	/// Códigos de los módulos del sistema (mismos códigos que Contagestor).
	/// </summary>
	public static class Areas
	{
		public const string Configuracion = "M000";
		public const string Stock = "M003";

		public static Dictionary<int, string> ObtenerModulos()
		{
			return new Dictionary<int, string>
			{
				{ 1, Configuracion },
				{ 2, Stock }
			};
		}

		public static Dictionary<string, string> ObtenerNombresModulos()
		{
			return new Dictionary<string, string>
			{
				{ Configuracion, nameof(Configuracion) },
				{ Stock, nameof(Stock) }
			};
		}
	}
}
