using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventosAPI.Gestion;
using EventosAPI.DTOs;

namespace EventosAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class InscripcionController : ControllerBase
{
    private readonly InscripcionGestion _gestion;

    public InscripcionController(InscripcionGestion gestion) => _gestion = gestion;

    private int ObtenerIdUsuario() => 
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost("inscribirse")]
    public async Task<IActionResult> Inscribirse([FromBody] InscripcionCrearDTO dto)
    {
        var resultado = await _gestion.InscribirseAEvento(dto.EventoId, ObtenerIdUsuario(), dto.ReferenciaPago);
        return resultado.Exito ? Ok(new { resultado.Mensaje }) : BadRequest(resultado.Mensaje);
    }

    [HttpDelete("desinscribirse/{idEvento}")]
    public async Task<IActionResult> Desinscribirse(int idEvento)
    {
        var resultado = await _gestion.DesinscribirseDeEvento(idEvento, ObtenerIdUsuario());
        return resultado.Exito ? Ok(new { resultado.Mensaje }) : BadRequest(resultado.Mensaje);
    }

    [HttpGet("mis-inscripciones")]
    public async Task<ActionResult<List<EventoListaDTO>>> GetMisInscripciones()
    {
        var lista = await _gestion.ObtenerInscripcionesPorUsuario(ObtenerIdUsuario());
        return Ok(lista);
    }

    [HttpPatch("asistencia/{idInscripcion}")]
    public async Task<IActionResult> AlternarAsistencia(int idInscripcion)
    {
        var resultado = await _gestion.AlternarAsistencia(idInscripcion, ObtenerIdUsuario());
        return resultado.Exito ? Ok(new { resultado.Mensaje }) : BadRequest(resultado.Mensaje);
    }

    [HttpGet("asistentes/{idEvento}")]
    public async Task<IActionResult> GetAsistentes(int idEvento)
    {
        var (asistentes, error) = await _gestion.ObtenerAsistentes(idEvento, ObtenerIdUsuario());
        if (error != null) return BadRequest(error);
        return Ok(asistentes);
    }

    [HttpPost("confirmar-pago")]
    public async Task<IActionResult> ConfirmarPago([FromBody] ConfirmarPagoDTO dto)
    {
        var resultado = await _gestion.ConfirmarPago(dto, ObtenerIdUsuario());
        return resultado.Exito ? Ok(new { resultado.Mensaje }) : BadRequest(resultado.Mensaje);
    }
        
    [HttpGet("esta-inscrito/{idEvento}")]
    public async Task<ActionResult<bool>> GetEstaInscrito(int idEvento)
    {
        var inscrito = await _gestion.EstaInscrito(idEvento, ObtenerIdUsuario());
        return Ok(inscrito);
    }


}