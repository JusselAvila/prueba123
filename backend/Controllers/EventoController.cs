using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims;             
using EventosAPI.Gestion;
using EventosAPI.DTOs;

namespace EventosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class EventoController (EventoGestion _gestionEventos): ControllerBase
{
  
    private int GetUsuarioId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    private IActionResult RespuestaResultado(bool exito, string mensaje) => exito ? Ok(new { mensaje }) : BadRequest(new { mensaje }); 

    [HttpPost("crear")]
    public async Task<IActionResult> Crear(EventoCrearDTO e)
    {
        var id = GetUsuarioId();

        if (id == 0) return Unauthorized(new { mensaje = "Token inválido." });
        
        var resultado = await _gestionEventos.CrearEvento(e, id);
        
        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, EventoCrearDTO solicitud)
    {
        var resultado = await _gestionEventos.ActualizarEvento(id, solicitud, GetUsuarioId());

        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var detalle = await _gestionEventos.ObtenerDetalleEvento(id);

        if (detalle == null) return NotFound(new { mensaje = "Evento no encontrado." });
        
        return Ok(detalle);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _gestionEventos.EliminarEvento(id, GetUsuarioId());

        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }

    [HttpGet("mis-eventos")]
    public async Task<IActionResult> ObtenerMisEventos() => Ok(await _gestionEventos.ObtenerEventosPorUsuario(GetUsuarioId()));

    [HttpGet("cartelera")]
    [AllowAnonymous] 
    public async Task<IActionResult> ObtenerCartelera() => 

        Ok(await _gestionEventos.ObtenerCarteleraDisponible());
}