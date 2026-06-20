using EventosAPI.Data;
using EventosAPI.Models;
using EventosAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventosAPI.Gestion;

public class InscripcionGestion
{
    private readonly AppDbContext _contexto;

    public InscripcionGestion(AppDbContext contexto) => _contexto = contexto;

    
    private async Task<Evento?> ObtenerEventoConInscripciones(int idEvento)
    {
        return await _contexto.Eventos.Include(e => e.Inscripciones).FirstOrDefaultAsync(e => e.EventoId == idEvento);
    }    

    private async Task<Inscripcion?> ObtenerInscripcionConEventoYUsuario(int idInscripcion)
    {
        return await _contexto.Inscripciones.Include(i => i.Evento).Include(i => i.Usuario).FirstOrDefaultAsync(i => i.InscripcionId == idInscripcion);
    }

    public async Task<(bool Exito, string Mensaje)> InscribirseAEvento(int idEvento, int UsuarioId, string? referenciaPago = null)
    {
        var ExisteEvento = await ObtenerEventoConInscripciones(idEvento);

        if (ExisteEvento == null) return (false, "El evento no existe.");

        if (ExisteEvento.UsuarioId == UsuarioId) return (false, "No puedes inscribirte a un evento que tú mismo creaste.");

        if (ExisteEvento.Inscripciones.Any(i => i.UsuarioId == UsuarioId)) return (false, "Ya estás inscrito en este evento.");

        if (ExisteEvento.Inscripciones.Count >= ExisteEvento.AforoMaximo) return (false, "Lo sentimos, no hay cupos disponibles.");

        try
        {
            var nuevaInscripcion = new Inscripcion
            {
                EventoId = idEvento,
                UsuarioId = UsuarioId,
                FechaInscripcion = DateTime.Now,
                PagoConfirmado = ExisteEvento.EsGratuito, 
                ReferenciaPago = referenciaPago
            };

            _contexto.Inscripciones.Add(nuevaInscripcion);
            await _contexto.SaveChangesAsync();
            
            return (true, ExisteEvento.EsGratuito ? "Inscripción exitosa." : "Inscripción registrada. Pendiente de pago.");
        }
        catch (Exception ex)
        {
            return (false, "Ocurrió un error al procesar tu inscripción. Por favor, intenta de nuevo más tarde." + ex.Message);
        }
    }
    public async Task<(bool Exito, string Mensaje)> DesinscribirsePorInscripcion(int idInscripcion, int idUsuario)
    {
        var inscripcion = await _contexto.Inscripciones.Include(i => i.Evento).FirstOrDefaultAsync(i => i.InscripcionId == idInscripcion && i.UsuarioId == idUsuario);

        if (inscripcion == null) return (false, "Inscripción no encontrada o no pertenece al usuario.");

        string mensajeAdicional = "";
        if (inscripcion.PagoConfirmado && !inscripcion.Evento.EsGratuito)
        {
            mensajeAdicional = " Se ha iniciado el proceso de reembolso de " + inscripcion.Evento.Precio + " Bs.";
        }

        try 
        {
            _contexto.Inscripciones.Remove(inscripcion);
            await _contexto.SaveChangesAsync();
            return (true, "Te has desinscrito correctamente del evento." + mensajeAdicional);
        }
        catch (Exception ex)
        {
            return (false, "Error al intentar desinscribirse: " + ex.Message);
        }
    }

    public async Task<(bool Exito, string Mensaje)> ConfirmarPago(ConfirmarPagoDTO d, int UsuarioId)
    {
        var inscripcion = await _contexto.Inscripciones.FirstOrDefaultAsync(i => i.InscripcionId == d.InscripcionId);

        if (inscripcion == null) return (false, "Inscripción no encontrada.");

        if (inscripcion.UsuarioId != UsuarioId) 
            return (false, "No puedes confirmar el pago de una inscripción que no es tuya.");

        try 
        {
            inscripcion.PagoConfirmado = true;

            inscripcion.ReferenciaPago = d.Referencia;

            await _contexto.SaveChangesAsync();

            return (true, "El pago ha sido confirmado exitosamente.");
        }
        catch (Exception ex)
        {
            return (false, "Ocurrió un error al confirmar el pago. Intenta más tarde." + ex.Message);
        }
    }


    public async Task<(bool Exito, string Mensaje)> AlternarAsistencia(int idInscripcion, int UsuarioId)
    {
        var inscripcion = await ObtenerInscripcionConEventoYUsuario(idInscripcion);

        if (inscripcion == null) return (false, "Inscripción no encontrada.");
            
        if (inscripcion.Evento.UsuarioId != UsuarioId) return (false, "No tienes permisos para modificar la asistencia de este evento.");

        inscripcion.Asistio = !inscripcion.Asistio;
        
        await _contexto.SaveChangesAsync();
        
        string estado = inscripcion.Asistio ? "marcado como asistente" : "marcado como ausente";

        return (true, "Su Asistencia Actualizada");
    }

    public async Task<(List<AsistenciaDTO>? Asistentes, string? Error)> ObtenerAsistentes(int EventoId, int UsuarioId)
    {   
        bool esCreador = await _contexto.Eventos.AnyAsync(e => e.EventoId == EventoId && e.UsuarioId == UsuarioId);

        if (!esCreador) return (null, "Evento no encontrado o no tienes permisos.");

        var asistentes = await _contexto.Inscripciones
            .Where(i => i.EventoId == EventoId)
            .Select(i => new AsistenciaDTO 
            {
                InscripcionId = i.InscripcionId,
                NombreUsuario = i.Usuario.Nombre, 
                CorreoUsuario = i.Usuario.Correo,
                Asistio = i.Asistio,
                PagoConfirmado = i.PagoConfirmado
            })
            .ToListAsync();

        return (asistentes, null);
    }
      public async Task<List<InscripcionDetalleDTO>> ObtenerInscripcionesPorUsuario(int idUsuario)
    {
        return await _contexto.Inscripciones.Where(i => i.UsuarioId == idUsuario).Select(i => new InscripcionDetalleDTO
            {
                InscripcionId = i.InscripcionId,
                NombreEvento = i.Evento.Nombre,
                FechaInscripcion = i.FechaInscripcion,
                PagoConfirmado = i.PagoConfirmado,
                Estado = !i.PagoConfirmado
                    ? "Pendiente de pago"
                    : i.Asistio
                        ? "Asistió"
                        : "Confirmada"
            }).ToListAsync();
    }
    public async Task<bool> EstaInscrito(int idEvento, int idUsuario)
    {
        return await _contexto.Inscripciones.AnyAsync(i => i.EventoId == idEvento && i.UsuarioId == idUsuario);
    }

 
    
}