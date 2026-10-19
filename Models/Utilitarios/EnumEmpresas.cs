namespace InventoryManagement.Models.Utilitarios
{
	public enum Empresas : short
	{
		Desconocida = 0,
		Comercial = 1,
		Servicios = 2,
		Industrial = 3
	}

	public enum Estatus : short
	{
		ACTIVO = 1,
		INACTIVO = 2,
		SUSPENDIDO = 3,
		REVISION = 4,
		ESPERA = 5,
	}
}
