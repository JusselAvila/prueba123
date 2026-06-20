using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using EventosAPI.Gestion;
using EventosAPI.DTOs;

namespace EventosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController (AuthGestion _gestion, TokenGestion _tokenGestion): ControllerBase 
{
    [HttpPost("registro")]
    public async Task<IActionResult> Registrar(RegistroDTO request)
    {
        var resultado = await _gestion.RegistrarUsuario(request);
        
        if (!resultado.Exito) return BadRequest(resultado.Mensaje);

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        var (usuario, mensaje) = await _gestion.ValidarLogin(request);
        
        if (usuario == null) return Unauthorized(mensaje);

        var token = _tokenGestion.GenerarToken(usuario.UsuarioId, usuario.Nombre, usuario.Rol);

        return Ok(new LoginRespuestaDTO
        {
            Token = token,
            UsuarioId = usuario.UsuarioId,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol
        });
    }
}