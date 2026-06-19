using System.ComponentModel.DataAnnotations;

namespace EventosAPI.Models;

public class Usuario
{
    [Key] 
    public int UsuarioId { get; set; }

    [Required] 
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress] 
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string ContraseñaHash { get; set; } = string.Empty;
    
    public string Rol { get; set; } = "Usuario";

    public List<Evento> EventosCreados { get; set; } = new();
    public List<Inscripcion> Inscripciones { get; set; } = new();
}