using System.ComponentModel.DataAnnotations;

namespace EventosAPI.DTOs;

public class ConfirmarPagoDTO 
{
    [Required(ErrorMessage = "El ID de la inscripción es obligatorio")]
    public int InscripcionId { get; set; }

    [Required(ErrorMessage = "La referencia de pago es obligatoria")]
    [StringLength(255, ErrorMessage = "La referencia no puede exceder los 255 caracteres")]
    public string Referencia { get; set; } = string.Empty; 
}