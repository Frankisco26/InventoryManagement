using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models.Identidad
{
	public class Usuario : IdentityUser<int>
	{
		public string Name { get; set; } = string.Empty;
		public int? UsuarioCreacionId { get; set; }
		public int? UsuarioModificacionId { get; set; }
		public DateTime? FechaCreacion { get; set; }
		public DateTime? FechaModificacion { get; set; }
		public string? IdentificacionEmpresaPadre { get; set; }

		/// <summary>
		/// Identificación (RUC) de la empresa principal del usuario
		/// </summary>
		public string? Empresa { get; set; }
	}
}
