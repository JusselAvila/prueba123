namespace EventosAPI.DTOs;

public class AsistenciaDTO
{
    public int InscripcionId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string CorreoUsuario { get; set; } = string.Empty;
    public bool Asistio { get; set; }
    public bool PagoConfirmado { get; set; }
}