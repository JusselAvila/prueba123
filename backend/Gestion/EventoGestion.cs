using EventosAPI.Data;
using EventosAPI.Models;
using EventosAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventosAPI.Gestion;
public class EventoGestion
{
    private readonly AppDbContext _contexto;
    public EventoGestion(AppDbContext contexto) => _contexto = contexto;
    private IQueryable<EventoListaDTO> ProyectarALista()
    {
    return _contexto.Eventos.Select(e => new EventoListaDTO {
        EventoId = e.EventoId,
        Nombre = e.Nombre,
        Categoria = e.Categoria,
        Fecha = e.Fecha,
        Horario = e.Horario,
        AforoMaximo = e.AforoMaximo,
        InscritosActuales = e.Inscripciones.Count,
        EsGratuito = e.EsGratuito,
        Precio = e.Precio,
        UrlImagen = e.UrlImagen
    });

    }

    private void MapearEvento(Evento evento, EventoCrearDTO e)
    {
        evento.Nombre = e.Nombre;
        evento.Descripcion = e.Descripcion;
        evento.AforoMaximo = e.AforoMaximo;
        evento.Fecha = e.Fecha;
        evento.Horario = e.Horario;
        evento.Ubicacion = e.Ubicacion;
        evento.Direccion = e.Direccion;
        evento.EsEventoGrupal = e.EsEventoGrupal;
        evento.EsGratuito = e.EsGratuito;
        evento.Precio = e.Precio;
        evento.Categoria = e.Categoria;
        evento.UrlImagen = e.UrlImagen;
    }

    public async Task<EventoListaDetalleDTO?> ObtenerDetalleEvento(int EventoId)
    {
        return await _contexto.Eventos
            .Include(e => e.Creador) 
            .Where(e => e.EventoId == EventoId)
            .Select(e => new EventoListaDetalleDTO {
                EventoId = e.EventoId,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion, 
                Fecha = e.Fecha,
                Horario = e.Horario,
                Ubicacion = e.Ubicacion,
                Direccion = e.Direccion,
                AforoMaximo = e.AforoMaximo,
                InscritosActuales = e.Inscripciones.Count,
                EsEventoGrupal = e.EsEventoGrupal,
                EsGratuito = e.EsGratuito,
                Precio = e.Precio,
                Categoria = e.Categoria,
                UrlImagen = e.UrlImagen,
                NombreOrganizador = e.Creador.Nombre 
            })
            .FirstOrDefaultAsync();
    }
    private (bool Exito, string Mensaje) EsEventoValido(EventoCrearDTO e)
    {
        if (string.IsNullOrWhiteSpace(e.Nombre)) 
            return (false, "El nombre del evento es obligatorio.");
        
        if (string.IsNullOrWhiteSpace(e.Categoria))
            return (false, "La categoría es obligatoria.");

        if (e.Fecha.Date < DateTime.Today) 
            return (false, "La fecha no puede ser en el pasado.");
        
        if (e.AforoMaximo <= 0 || e.AforoMaximo > 10000) 
            return (false, "El aforo debe ser un número entre 1 y 10,000.");
        
        if (!e.EsGratuito && e.Precio <= 0) 
            return (false, "Si el evento es de pago, el precio debe ser mayor a 0.");

        return (true, string.Empty);
    }

    public async Task<(bool Exito, string Mensaje)> CrearEvento(EventoCrearDTO e, int UsuarioId)
    {
        var validacion = EsEventoValido(e);
        if (!validacion.Exito) return (false, validacion.Mensaje);

        try 
        {
            var nuevoEvento = new Evento { UsuarioId = UsuarioId };
            MapearEvento(nuevoEvento, e);
            
            _contexto.Eventos.Add(nuevoEvento);
            await _contexto.SaveChangesAsync(); 
            
            return (true, "Evento creado correctamente.");
        }
        catch (Exception ex) 
        {
            return (false, "Error al guardar en la base de datos: " + ex.Message);
        }
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarEvento(int EventoId, EventoCrearDTO e, int UsuarioId)
    {
        var evento = await _contexto.Eventos.FirstOrDefaultAsync(x => x.EventoId == EventoId && x.UsuarioId == UsuarioId);

        if(evento == null) return (false, "Evento no encontrado");

        var validacion = EsEventoValido(e);

        if (!validacion.Exito) return (false, validacion.Mensaje);

        try
        {
            MapearEvento(evento, e);

            await _contexto.SaveChangesAsync();

            return (true, "Evento actualizado correctamente.");
        } 
        catch (Exception ex)
        {
            return (false, "Error al guardar en la base de datos: " + ex.Message);
        }
    }

   public async Task<(bool Exito, string Mensaje)> EliminarEvento(int EventoId, int UsuarioId)
    {
        var evento = await _contexto.Eventos.Include(e => e.Inscripciones).FirstOrDefaultAsync(x => x.EventoId == EventoId && x.UsuarioId == UsuarioId);

        if (evento == null) return (false, "Evento no encontrado o no autorizado.");

        if (evento.Inscripciones.Any()) return (false, "No se puede eliminar porque ya gente se inscribio");

        _contexto.Eventos.Remove(evento);

        await _contexto.SaveChangesAsync();

        return (true, "Evento eliminado correctamente.");
    }

    public async Task<List<EventoListaDTO>> ObtenerCarteleraDisponible()
    {
        return await ProyectarALista().Where(e => e.InscritosActuales < e.AforoMaximo).ToListAsync();
    }

    public async Task<bool> EsCreadorDelEvento(int EventoId, int UsuarioId)
    {
        return await _contexto.Eventos.AnyAsync(x => x.EventoId == EventoId && x.UsuarioId == UsuarioId);
    }

    public async Task<List<EventoListaDTO>> ObtenerEventosPorUsuario(int idUsuario)
    {
        return await ProyectarALista().Where(e => _contexto.Eventos.Any(ev => ev.EventoId == e.EventoId && ev.UsuarioId == idUsuario)).ToListAsync();
    }

}