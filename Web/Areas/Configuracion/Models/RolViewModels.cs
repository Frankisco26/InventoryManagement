using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Areas.Configuracion.Models
{
	public class RolViewModels
	{
		public int Id { get; set; }

		[Display(Name = "Empresa")]
		public string? NombreEmpresa { get; set; }

		public string IdentificacionEmpresa { get; set; } = string.Empty;
		public string? IdentificacionEmpresaPadre { get; set; }

		[Required]
		[Display(Name = "Nombre")]
		public string Nombre { get; set; } = string.Empty;

		public int? UsuarioId { get; set; }

		public bool PermiteCrear { get; set; }
		public bool PermiteEditar { get; set; }
		public bool PermiteAnular { get; set; }
	}

	public class RolPantallaViewModel
	{
		public string Nombre { get; set; } = string.Empty;
		public string? Area { get; set; }
		public string? Controlador { get; set; }
		public string Modulo { get; set; } = string.Empty;
		public bool Activa { get; set; }
		public short Orden { get; set; }
	}
}
