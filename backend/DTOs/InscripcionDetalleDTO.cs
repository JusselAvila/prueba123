namespace EventosAPI.DTOs;

public class InscripcionDetalleDTO
{
    public int InscripcionId { get; set; }
    public string NombreEvento { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public bool PagoConfirmado { get; set; }
    public string Estado { get; set; } = string.Empty; 
}