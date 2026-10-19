using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Areas.Configuracion.Models
{
	public class UsuarioIndexViewModel
	{
		public int Id { get; set; }

		public string? IdentificacionEmpresa { get; set; }

		[Display(Name = "Empresa")]
		public string? NombreEmpresa { get; set; }

		[Display(Name = "Usuario")]
		public string? Cuenta { get; set; }

		[Display(Name = "Correo electrónico")]
		[EmailAddress]
		public string? Email { get; set; }

		public string? Nombre { get; set; }

		/// <summary>
		/// Contraseña inicial (al crear) o nueva contraseña (al editar). Si va vacía al crear se genera una.
		/// </summary>
		[Display(Name = "Contraseña")]
		public string? Clave { get; set; }

		[Display(Name = "Bloqueo Cuenta")]
		public bool BloqueoHabilitado { get; set; }

		[Display(Name = "Fin del Bloqueo")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
		public DateTimeOffset? FechaFin { get; set; }

		public string[] Roles { get; set; } = Array.Empty<string>();
	}
}
