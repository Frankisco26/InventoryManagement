using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Web.Controllers.Api
{
    /// <summary>
    /// Base de la API REST: autenticación JWT y empresa tomada del token.
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseApiController : ControllerBase
    {
        protected int EmpresaId => int.TryParse(User.FindFirstValue(JwtService.EmpresaIdClaim), out var id) ? id : 0;

        protected string UsuarioId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "sistema";
    }
}
