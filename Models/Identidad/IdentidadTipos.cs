using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models.Identidad
{
	public class UsuarioRol : IdentityUserRole<int>
	{
	}

	public class UsuarioReclamo : IdentityUserClaim<int>
	{
	}

	public class UsuarioAcceso : IdentityUserLogin<int>
	{
	}

	public class UsuarioToken : IdentityUserToken<int>
	{
	}

	public class RolReclamo : IdentityRoleClaim<int>
	{
	}
}
