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


    private void MapearEvento(Evento evento, EventoCrearDTO solicitud)
    {
    evento.Nombre = solicitud.Nombre;
    evento.Descripcion = solicitud.Descripcion;
    evento.AforoMaximo = solicitud.AforoMaximo;
    evento.Fecha = solicitud.Fecha;
    evento.Horario = solicitud.Horario;
    evento.Ubicacion = solicitud.Ubicacion;
    evento.Direccion = solicitud.Direccion;
    evento.EsEventoGrupal = solicitud.EsEventoGrupal;
    evento.EsGratuito = solicitud.EsGratuito;
    evento.Precio = solicitud.Precio;
    evento.Categoria = solicitud.Categoria;
    evento.UrlImagen = solicitud.UrlImagen;
    }
    public async Task<EventoListaDetalleDTO?> ObtenerDetalleEvento(int idEvento)
    {
        return await _contexto.Eventos
            .Include(e => e.Creador) // Cambiado: ahora usamos Creador
            .Where(e => e.EventoId == idEvento)
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
                NombreOrganizador = e.Creador.Nombre // Cambiado: ahora usamos e.Creador.Nombre
            })
            .FirstOrDefaultAsync();
    }
    private bool EsEventoValido(EventoCrearDTO solicitud, out string mensaje)
    {
        // LOG PARA DEBUGUEAR (Míralo en la consola de tu terminal de .NET)
        Console.WriteLine($"DEBUG: Fecha recibida: {solicitud.Fecha.Date}, Hoy: {DateTime.Today}");

        // Cambia la comparación a algo más flexible para pruebas
        if (solicitud.Fecha.Date < DateTime.Today.AddDays(-1)) 
        {
            mensaje = $"La fecha recibida ({solicitud.Fecha.Date}) es anterior a hoy ({DateTime.Today}).";
            return false;
        }

        if (solicitud.AforoMaximo <= 0)
        {
            mensaje = "El aforo máximo debe ser mayor a cero.";
            return false;
        }

        mensaje = string.Empty;
        return true;
    }
    public async Task<(bool Exito, string Mensaje)> CrearEvento(EventoCrearDTO solicitud, int idUsuario)
    {
        try 
        {
            if (!EsEventoValido(solicitud, out var mensajeError)) return (false, mensajeError);
            var nuevoEvento = new Evento { UsuarioId = idUsuario };
            MapearEvento(nuevoEvento, solicitud);
            _contexto.Eventos.Add(nuevoEvento);
            await _contexto.SaveChangesAsync();
            return (true, "Evento creado correctamente.");
        }
        catch (Exception)
        {
            return (false, "Ocurrió un error inesperado al guardar el evento.");
        }
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarEvento(int idEvento, EventoCrearDTO solicitud, int idUsuario)
    {
        var evento = await _contexto.Eventos
            .FirstOrDefaultAsync(e => e.EventoId == idEvento && e.UsuarioId == idUsuario);

        if (evento == null) return (false, "Evento no encontrado.");
        if (!EsEventoValido(solicitud, out var mensajeError)) return (false, mensajeError);

        MapearEvento(evento, solicitud); 

        await _contexto.SaveChangesAsync();
        return (true, "Evento actualizado correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarEvento(int idEvento, int idUsuario)
    {
        var evento = await _contexto.Eventos
            .Include(e => e.Inscripciones) 
            .FirstOrDefaultAsync(e => e.EventoId == idEvento && e.UsuarioId == idUsuario);

        if (evento == null) return (false, "Evento no encontrado o no autorizado.");

        if (evento.Inscripciones != null && evento.Inscripciones.Any())
        {
            return (false, "No se puede eliminar el evento porque ya cuenta con inscritos.");
        }

        _contexto.Eventos.Remove(evento);
        await _contexto.SaveChangesAsync();
        return (true, "Evento eliminado correctamente.");
    }

    public async Task<List<EventoListaDTO>> ObtenerCarteleraDisponible()
    {
        return await ProyectarALista()
            .Where(e => e.InscritosActuales < e.AforoMaximo)
            .ToListAsync();
    }

    public async Task<bool> EsCreadorDelEvento(int idEvento, int idUsuario)
    {
        return await _contexto.Eventos.AnyAsync(e => e.EventoId == idEvento && e.UsuarioId == idUsuario);
    }
    
    public async Task<List<EventoListaDTO>> ObtenerEventosPorUsuario(int idUsuario)
    {
        return await ProyectarALista()
            .Where(e => _contexto.Eventos.Any(ev => ev.EventoId == e.EventoId && ev.UsuarioId == idUsuario))
            .ToListAsync();
    }
}