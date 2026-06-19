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
        return await _contexto.Eventos
            .Include(e => e.Inscripciones)
            .FirstOrDefaultAsync(e => e.EventoId == idEvento);
    }

    private async Task<Inscripcion?> ObtenerInscripcionConEventoYUsuario(int idInscripcion)
    {
        return await _contexto.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Usuario)
            .FirstOrDefaultAsync(i => i.InscripcionId == idInscripcion);
    }
    public async Task<(bool Exito, string Mensaje)> InscribirseAEvento(int idEvento, int idUsuario, string? referenciaPago = null)
    {
        var evento = await ObtenerEventoConInscripciones(idEvento);

        if (evento == null) return (false, "El evento no existe.");
        if (evento.Inscripciones.Any(i => i.UsuarioId == idUsuario)) return (false, "Ya estás inscrito en este evento.");
        if (evento.Inscripciones.Count >= evento.AforoMaximo) return (false, "Lo sentimos, no hay cupos disponibles.");

        var nuevaInscripcion = new Inscripcion
        {
            EventoId = idEvento,
            UsuarioId = idUsuario,
            FechaInscripcion = DateTime.Now,
            PagoConfirmado = evento.EsGratuito, 
            ReferenciaPago = referenciaPago
        };

        _contexto.Inscripciones.Add(nuevaInscripcion);
        await _contexto.SaveChangesAsync();
        
        return (true, evento.EsGratuito ? "Inscripción exitosa." : "Inscripción registrada. Pendiente de pago.");
    }

    public async Task<(bool Exito, string Mensaje)> DesinscribirseDeEvento(int idEvento, int idUsuario)
    {
        var inscripcion = await _contexto.Inscripciones
            .FirstOrDefaultAsync(i => i.EventoId == idEvento && i.UsuarioId == idUsuario);

        if (inscripcion == null) return (false, "No estás inscrito en este evento.");

        _contexto.Inscripciones.Remove(inscripcion);
        await _contexto.SaveChangesAsync();
        return (true, "Te has desinscrito correctamente del evento.");
    }
    public async Task<(bool Exito, string Mensaje)> ConfirmarPago(ConfirmarPagoDTO dto)
    {
        Console.WriteLine($"[DEBUG] Buscando InscripcionId: {dto.InscripcionId} con Ref: {dto.Referencia}");

        var inscripcion = await _contexto.Inscripciones.FindAsync(dto.InscripcionId);
        
        if (inscripcion == null) 
        {
            return (false, $"Inscripción {dto.InscripcionId} no encontrada.");
        }

        inscripcion.PagoConfirmado = true;
        // AQUÍ usamos la referencia real que viene del Frontend
        inscripcion.ReferenciaPago = dto.Referencia; 
        
        await _contexto.SaveChangesAsync();
        return (true, "El pago ha sido confirmado exitosamente.");
    }
    public async Task<(bool Exito, string Mensaje)> AlternarAsistencia(int idInscripcion, int idUsuarioLogueado)
    {
        var inscripcion = await ObtenerInscripcionConEventoYUsuario(idInscripcion);

        if (inscripcion == null) 
            return (false, "Inscripción no encontrada.");
            
        if (inscripcion.Evento.UsuarioId != idUsuarioLogueado)
            return (false, "No tienes permisos para modificar la asistencia de este evento.");

        inscripcion.Asistio = !inscripcion.Asistio;
        await _contexto.SaveChangesAsync();
        
        string estado = inscripcion.Asistio ? "marcado como asistente" : "marcado como ausente";
        return (true, $"Usuario {estado} correctamente.");
    }

    public async Task<(List<AsistenciaDTO>? Asistentes, string? Error)> ObtenerAsistentes(int idEvento, int idUsuarioLogueado)
    {
        var evento = await _contexto.Eventos.FirstOrDefaultAsync(e => e.EventoId == idEvento);
        
        if (evento == null) 
            return (null, "El evento no existe.");
            
        if (evento.UsuarioId != idUsuarioLogueado) 
            return (null, "No tienes permisos para ver los asistentes de este evento.");

        var asistentes = await _contexto.Inscripciones
            .Where(i => i.EventoId == idEvento)
            .Select(i => new AsistenciaDTO {
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
        return await _contexto.Inscripciones
            .Where(i => i.UsuarioId == idUsuario)
            .Select(i => new InscripcionDetalleDTO
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
            })
            .ToListAsync();
    }

    public async Task<bool> EstaInscrito(int idEvento, int idUsuario)
    {
        return await _contexto.Inscripciones
            .AnyAsync(i => i.EventoId == idEvento && i.UsuarioId == idUsuario);
    }



    public async Task<(bool Exito, string Mensaje)> ConfirmarPago(ConfirmarPagoDTO dto, int idUsuarioLogueado)
{
    var inscripcion = await _contexto.Inscripciones.FindAsync(dto.InscripcionId);

    if (inscripcion == null)
        return (false, $"Inscripción {dto.InscripcionId} no encontrada.");

    if (inscripcion.UsuarioId != idUsuarioLogueado)
        return (false, "No puedes confirmar el pago de una inscripción que no es tuya.");

    inscripcion.PagoConfirmado = true;
    inscripcion.ReferenciaPago = dto.Referencia;

    await _contexto.SaveChangesAsync();
    return (true, "El pago ha sido confirmado exitosamente.");
}
    
}