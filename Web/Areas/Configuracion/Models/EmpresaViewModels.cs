using InventoryManagement.Models.Utilitarios;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Areas.Configuracion.Models
{
	public class EmpresaIndexViewModel
	{
		public int Id { get; set; }
		public string Codigo { get; set; } = string.Empty;
		public string Identificacion { get; set; } = string.Empty;
		public string Nombre { get; set; } = string.Empty;
		public string? Telefono { get; set; }
		public string? Correo { get; set; }
		public string Estatus { get; set; } = string.Empty;
		public int Usuarios { get; set; }
		public string Modulos { get; set; } = string.Empty;
	}

	public class EmpresaEditViewModel
	{
		public int Id { get; set; }

		public string? Codigo { get; set; }

		[Required(ErrorMessage = "El RUC es obligatorio")]
		[StringLength(13, MinimumLength = 10, ErrorMessage = "El RUC debe tener entre 10 y 13 dígitos")]
		[Display(Name = "Ruc")]
		public string Identificacion { get; set; } = string.Empty;

		[Required(ErrorMessage = "El nombre es obligatorio")]
		public string Nombre { get; set; } = string.Empty;

		[Required(ErrorMessage = "La razón social es obligatoria")]
		[Display(Name = "Razón Social")]
		public string RazonSocial { get; set; } = string.Empty;

		[Display(Name = "Actividad económica")]
		public string? ActividadEcononica { get; set; }

		public Empresas Tipo { get; set; } = Empresas.Comercial;

		public Estatus Estatus { get; set; } = Estatus.ACTIVO;

		[Display(Name = "Identificación representante legal")]
		public string? IdentificacionRepresentanteLegal { get; set; }

		[Display(Name = "Representante legal")]
		public string? NombreRepresentanteLegal { get; set; }

		[Display(Name = "Lleva contabilidad")]
		public bool LlevaContable { get; set; }

		public string? Telefono { get; set; }

		public string? Direccion { get; set; }

		[Required(ErrorMessage = "El correo es obligatorio")]
		[EmailAddress(ErrorMessage = "El correo no es válido")]
		public string Correo { get; set; } = string.Empty;

		[Display(Name = "Persona de contacto")]
		public string? PersonaContacto { get; set; }

		public List<ModuloViewModel> Modulos { get; set; } = new();

		public static IEnumerable<SelectListItem> TiposEstatus() =>
			Enum.GetValues<Estatus>().Select(e => new SelectListItem(e.ToString(), e.ToString()));

		public static IEnumerable<SelectListItem> TiposEmpresas() =>
			Enum.GetValues<Empresas>().Where(e => e != Empresas.Desconocida).Select(e => new SelectListItem(e.ToString(), e.ToString()));
	}

	public class ModuloViewModel
	{
		public string Codigo { get; set; } = string.Empty;
		public string Nombre { get; set; } = string.Empty;
		public bool Activo { get; set; }
	}
}
