using System.ComponentModel.DataAnnotations;

namespace EventosAPI.DTOs;

public class InscripcionCrearDTO
{
    [Required(ErrorMessage = "El ID del evento es obligatorio")]
    public int EventoId { get; set; }
    public string? ReferenciaPago { get; set; } 
}