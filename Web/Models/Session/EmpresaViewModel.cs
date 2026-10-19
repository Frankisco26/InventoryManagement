namespace InventoryManagement.Web.Models.Session
{
	/// <summary>
	/// Empresa activa del usuario, con los módulos activos y las pantallas que su rol permite.
	/// Se guarda en la sesión con la clave "Empresa".
	/// </summary>
	public class EmpresaViewModel
	{
		public int Id { get; set; }
		public string Identificacion { get; set; } = string.Empty;
		public string Nombre { get; set; } = string.Empty;
		public bool PermiteCrear { get; set; }
		public bool PermiteEditar { get; set; }
		public bool PermiteAnular { get; set; }
		public List<string> Modulos { get; set; } = new();
		public List<PantallaViewModel> Pantallas { get; set; } = new();
	}

	public class PantallaViewModel
	{
		public string Nombre { get; set; } = string.Empty;
		public string? Controlador { get; set; }
		public string? Area { get; set; }
		public string Modulo { get; set; } = string.Empty;
		public string? Accion { get; set; }
		public string? Icon { get; set; }
		public int Orden { get; set; }
		public bool Activo { get; set; }
	}
}
