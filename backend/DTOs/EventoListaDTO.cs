namespace EventosAPI.DTOs;

public class EventoListaDTO
{
    public int EventoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Horario { get; set; } = string.Empty; 
    public int InscritosActuales { get; set; }
    public int AforoMaximo { get; set; }
    public bool EsGratuito { get; set; }            
    public decimal Precio { get; set; }  
    public string? UrlImagen { get; set; }
    
}