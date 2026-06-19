using System.ComponentModel.DataAnnotations;

namespace EventosAPI.Models;

public class Inscripcion
{
    [Key]
    public int InscripcionId { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now; 
    public bool Asistio { get; set; } = false;

    public bool PagoConfirmado { get; set; } = false; 
    public string? ReferenciaPago { get; set; } 

    public int UsuarioId { get; set; } 
    public Usuario Usuario { get; set; } = null!;

    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!; 
}