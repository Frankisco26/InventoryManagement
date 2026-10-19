namespace InventoryManagement.Models.Identidad
{
	public class RolPantalla
	{
		public int Id { get; set; }

		public int RolId { get; set; }
		public virtual Rol? Rol { get; set; }

		public int PantallaId { get; set; }
		public virtual Pantalla? Pantalla { get; set; }
	}
}
