using InventoryManagement.Models.Utilitarios;

namespace InventoryManagement.Data.Base
{
	/// <summary>
	/// Datos sembrados por las migraciones: empresa principal, pantallas, rol administrador
	/// y el usuario principal (MANTENIMIENTO).
	/// </summary>
	public static class DatosIniciales
	{
		public const int EmpresaPrincipalId = 1;
		public const string EmpresaPrincipalIdentificacion = "1250130620001";
		public const string EmpresaPrincipalNombre = "EMPRESA PRINCIPAL";

		public const int RolAdministradorId = 1;
		public const string RolAdministrador = "Administrador";

		public const int UsuarioPrincipalId = 1;

		public static readonly (int Id, string Nombre, short Orden, string Modulo, string Area, string Controlador)[] Pantallas =
		{
			(1, "Empresas", 1, Areas.Configuracion, "Configuracion", "Empresas"),
			(2, "Usuarios", 2, Areas.Configuracion, "Configuracion", "Usuarios"),
			(3, "Permisos", 3, Areas.Configuracion, "Configuracion", "Rol"),
			(4, "Productos", 1, Areas.Stock, "Stock", "Productos"),
			(5, "Movimientos", 2, Areas.Stock, "Stock", "Movimientos"),
		};

		/// <summary>
		/// Pantallas que solo administra la empresa principal; al crear una empresa nueva
		/// su rol Administrador no las recibe (igual que en Contagestor).
		/// </summary>
		public static readonly string[] PantallasExcluidas = { "Empresas", "Usuarios", "Permisos" };
	}
}
