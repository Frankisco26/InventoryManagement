using InventoryManagement.Models.Abstractos;
using InventoryManagement.Models.Utilitarios;

namespace InventoryManagement.Models.Administracion
{
	public class Empresa : EntidadCodificada
	{
		public string Nombre { get; set; } = string.Empty;
		public string Identificacion { get; set; } = string.Empty;
		public string RazonSocial { get; set; } = string.Empty;
		public string? ActividadEcononica { get; set; }
		public Utilitarios.Empresas Tipo { get; set; }
		public string? IdentificacionRepresentanteLegal { get; set; }
		public string? NombreRepresentanteLegal { get; set; }
		public Estatus Estatus { get; set; } = Estatus.ACTIVO;
		public bool LlevaContable { get; set; }
		public string? Telefono { get; set; }
		public string? Direccion { get; set; }
		public string? Correo { get; set; }
		public string? PersonaContacto { get; set; }
		public bool Principal { get; set; }
		public string? IdentificacionEmpresaPadre { get; set; }

		public IList<Modulo> Modulos { get; set; } = new List<Modulo>();
		public IList<Parametro> Parametros { get; set; } = new List<Parametro>();
		public IList<Product> Productos { get; set; } = new List<Product>();
	}
}
