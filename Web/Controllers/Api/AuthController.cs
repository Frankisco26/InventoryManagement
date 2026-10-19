using InventoryManagement.Models.Identidad;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly EmpresasUsuario _empresasUsuario;
        private readonly JwtService _jwtService;

        public AuthController(UserManager<Usuario> userManager, EmpresasUsuario empresasUsuario, JwtService jwtService)
        {
            _userManager = userManager;
            _empresasUsuario = empresasUsuario;
            _jwtService = jwtService;
        }

        public class TokenRequest
        {
            [Required]
            public string Usuario { get; set; } = string.Empty;

            [Required]
            public string Clave { get; set; } = string.Empty;

            /// <summary>
            /// RUC de la empresa; si se omite se usa la empresa predeterminada del usuario
            /// </summary>
            public string? Empresa { get; set; }
        }

        /// <summary>
        /// Obtener un token JWT para consumir la API sobre una empresa del usuario
        /// </summary>
        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] TokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Usuario);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Clave))
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            if (user.LockoutEnabled || (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now))
                return Unauthorized(new { message = $"El usuario {user.UserName} se encuentra bloqueado." });

            var (empresa, _) = await _empresasUsuario.CargarEmpresaAsync(user.Id, request.Empresa);
            if (empresa == null)
                return Unauthorized(new { message = "El usuario no tiene acceso a la empresa indicada." });

            return Ok(new
            {
                token = _jwtService.GenerateToken(user, empresa),
                empresa = new { empresa.Id, empresa.Identificacion, empresa.Nombre }
            });
        }
    }
}
