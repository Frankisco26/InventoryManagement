using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagement.Web.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class AccessDeniedModel : PageModel
	{
		public void OnGet()
		{
		}
	}
}
