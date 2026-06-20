using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventosAPI.Gestion;
using EventosAPI.DTOs;

namespace EventosAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class InscripcionController (InscripcionGestion _gestion): ControllerBase
{
    private int GetUsuarioId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    private IActionResult RespuestaResultado(bool exito, string mensaje) => exito ? Ok(new { mensaje }) : BadRequest(new { mensaje });
    
    [HttpPost("inscribirse")]
    public async Task<IActionResult> Inscribirse([FromBody] InscripcionCrearDTO dto)
    {
        var resultado = await _gestion.InscribirseAEvento(dto.EventoId, GetUsuarioId(), dto.ReferenciaPago);

        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }

    [HttpDelete("desinscribirse/{idInscripcion}")]
    public async Task<IActionResult> Desinscribirse(int idInscripcion)
    {
    var resultado = await _gestion.DesinscribirsePorInscripcion(idInscripcion, GetUsuarioId());

    return RespuestaResultado(resultado.Exito, resultado.Mensaje);
}
    [HttpGet("mis-inscripciones")]
    public async Task<ActionResult<List<EventoListaDTO>>> GetMisInscripciones()
    {
        var lista = await _gestion.ObtenerInscripcionesPorUsuario(GetUsuarioId());
        
        return Ok(lista);
    }

    [HttpPatch("asistencia/{idInscripcion}")]
    public async Task<IActionResult> AlternarAsistencia(int idInscripcion)
    {
        var resultado = await _gestion.AlternarAsistencia(idInscripcion, GetUsuarioId());

        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }

    [HttpGet("asistentes/{idEvento}")]
    public async Task<IActionResult> GetAsistentes(int idEvento)
    {
        var (asistentes, error) = await _gestion.ObtenerAsistentes(idEvento, GetUsuarioId());
        if (error != null) return BadRequest(error);
        return Ok(asistentes);
    }

    [HttpPost("confirmar-pago")]
    public async Task<IActionResult> ConfirmarPago([FromBody] ConfirmarPagoDTO dto)
    {
        var resultado = await _gestion.ConfirmarPago(dto, GetUsuarioId());

        return RespuestaResultado(resultado.Exito, resultado.Mensaje);
    }
        
    [HttpGet("esta-inscrito/{idEvento}")]
    public async Task<ActionResult<bool>> GetEstaInscrito(int idEvento)
    {
        var inscrito = await _gestion.EstaInscrito(idEvento, GetUsuarioId());
        return Ok(inscrito);
    }
}