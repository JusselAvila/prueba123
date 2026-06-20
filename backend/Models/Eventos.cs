using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventosAPI.Models;

public class Evento 
{
    [Key] 
    public int EventoId { get; set; }

    [Required] 
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Descripcion { get; set; } = string.Empty;
    
    [Required]
    public int AforoMaximo { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public string Horario { get; set; } = string.Empty;

    [Required]
    public string Ubicacion { get; set; } = string.Empty;

    [Required]
    public string Direccion { get; set; } = string.Empty;

    public bool EsEventoGrupal { get; set; } 
    
    public bool EsGratuito { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }  

    public string? UrlImagen { get; set; } 

    [Required]
    public string Categoria { get; set; } = string.Empty;

    public int UsuarioId { get; set; } 
    public Usuario Creador { get; set; } = null!; 
    
    public List<Inscripcion> Inscripciones { get; set; } = new();
}