using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models.Identidad
{
	public class Rol : IdentityRole<int>
	{
		public int? UsuarioCreacionId { get; set; }
		public int? UsuarioModificacionId { get; set; }
		public DateTime? FechaCreacion { get; set; }
		public DateTime? FechaModificacion { get; set; }
		public IList<RolPantalla> Pantallas { get; set; } = new List<RolPantalla>();
		public string? IdentificacionEmpresaPadre { get; set; }

		/// <summary>
		/// Identificación (RUC) de la empresa a la que pertenece el rol
		/// </summary>
		public string Empresa { get; set; } = string.Empty;
		public bool PermiteCrear { get; set; } = true;
		public bool PermiteEditar { get; set; } = true;
		public bool PermiteAnular { get; set; } = true;
	}
}
