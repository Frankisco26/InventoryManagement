using InventoryManagement.Models.Abstractos;

namespace InventoryManagement.Models.Administracion
{
	/// <summary>
	/// Entidad para almacenar los modulos del sistema habilitados por empresa
	/// </summary>
	public class Modulo : EntidadCodificada
	{
		public Modulo()
		{
		}

		public Modulo(string codigo, bool activo = true) : this()
		{
			Codigo = codigo;
			Activo = activo;
		}

		public bool Activo { get; set; }

		public int EmpresaId { get; set; }
		public virtual Empresa? Empresa { get; set; }
	}
}
