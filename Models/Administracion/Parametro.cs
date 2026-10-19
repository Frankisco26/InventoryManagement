using InventoryManagement.Models.Abstractos;

namespace InventoryManagement.Models.Administracion
{
	public class Parametro : EntidadCodificada
	{
		public int EmpresaId { get; set; }
		public virtual Empresa? Empresa { get; set; }

		public string Nombre { get; set; } = string.Empty;
		public string? Descripcion { get; set; }
		public string? Valor { get; set; }
		public string? Valor2 { get; set; }
	}
}
