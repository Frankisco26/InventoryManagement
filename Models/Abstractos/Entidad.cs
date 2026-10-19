namespace InventoryManagement.Models.Abstractos
{
	/// <summary>
	/// Entidad abstracta con clave entera y campos de auditoría
	/// </summary>
	public abstract class Entidad
	{
		/// <summary>
		/// Devuelve o establece el Id de la entidad
		/// </summary>
		public int Id { get; set; }

		public int? UsuarioCreacionId { get; set; }
		public int? UsuarioModificacionId { get; set; }
		public DateTime? FechaCreacion { get; set; }
		public DateTime? FechaModificacion { get; set; }
	}

	/// <summary>
	/// Entidad abstracta con clave entera y codificación
	/// </summary>
	public abstract class EntidadCodificada : Entidad
	{
		/// <summary>
		/// Devuelve o establece el código de la entidad
		/// </summary>
		public string Codigo { get; set; } = string.Empty;
	}
}
