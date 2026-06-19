namespace EventosAPI.DTOs;

public class EventoListaDetalleDTO
{
    public int EventoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int AforoMaximo { get; set; }
    public int InscritosActuales { get; set; }
    public bool EsEventoGrupal { get; set; }
    public bool EsGratuito { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? UrlImagen { get; set; }
    public string NombreOrganizador { get; set; } = string.Empty;
}