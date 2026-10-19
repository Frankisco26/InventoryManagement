using InventoryManagement.Data.Base;
using InventoryManagement.Web.Models.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    [Authorize]
    public abstract class BaseMvcController : Controller
    {
        internal readonly ContextoPrincipal _context;
        internal readonly ILogger _logger;

        protected BaseMvcController(ContextoPrincipal context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger(GetType());
        }

        /// <summary>
        /// Empresa activa de la sesión
        /// </summary>
        protected EmpresaViewModel EmpresaSesion => User.GetEmpresaSesion();

        protected int UsuarioId => User.GetUserId<int>();
    }
}
