using InventoryManagement.Models.Utilitarios;

namespace InventoryManagement.Models.Identidad
{
	public class Pantalla
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public short Order { get; set; } = 1;

		#region Ruta MVC

		public string Module { get; set; } = Areas.Configuracion;
		public string? Area { get; set; }
		public string? Controller { get; set; }
		public string? Action { get; set; }

		#endregion

		public bool Active { get; set; } = true;

		public IList<RolPantalla> Roles { get; set; } = new List<RolPantalla>();
	}
}
