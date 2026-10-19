using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Web.Models.Session;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.Web.Services;

public class JwtService
{
    /// <summary>
    /// Claim con el Id de la empresa sobre la que trabaja el token
    /// </summary>
    public const string EmpresaIdClaim = "empresa_id";

    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Genera un token para la API limitado a una empresa del usuario.
    /// </summary>
    public string GenerateToken(Usuario user, EmpresaViewModel empresa)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.GivenName, user.Name),
            new Claim(CustomClaimTypes.RucEmpresa, empresa.Identificacion),
            new Claim(EmpresaIdClaim, empresa.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(
                    jwtSettings["DurationInMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
