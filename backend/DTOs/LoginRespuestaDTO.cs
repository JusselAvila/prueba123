namespace EventosAPI.DTOs;


public class LoginRespuestaDTO
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}