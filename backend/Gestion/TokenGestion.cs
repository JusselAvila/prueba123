using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventosAPI.Gestion;

public class TokenGestion
{
    private readonly string _key;
    public TokenGestion(IConfiguration config)
    {
        _key = config["Jwt:Key"] 
               ?? throw new InvalidOperationException("La clave JWT no está configurada en appsettings.json");
    }

    public string GenerarToken(int usuarioId, string nombre, string rol)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Role, rol)
        };

        // Usamos la clave que leímos del archivo de configuración
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}