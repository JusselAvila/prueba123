using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using EventosAPI.Gestion;
using EventosAPI.DTOs;
using EventosAPI.Data;

namespace EventosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase 
{
    private readonly AuthGestion _gestion;
    private readonly TokenGestion _tokenGestion; // 1. Declarar

    public AuthController(AppDbContext context, IConfiguration config) // 2. Recibir config
    {
        _gestion = new AuthGestion(context);
        _tokenGestion = new TokenGestion(config); 
    }

 
    [HttpPost("registro")]
    public async Task<IActionResult> Registrar(RegistroDTO request)
    {
        // Capturamos el resultado del objeto (Exito, Mensaje)
        var resultado = await _gestion.RegistrarUsuario(request);
        
        // Si falla, devolvemos el mensaje específico que viene de la lógica
        if (!resultado.Exito) return BadRequest(resultado.Mensaje);

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        // Usamos await porque ValidarLogin ahora es asíncrono
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