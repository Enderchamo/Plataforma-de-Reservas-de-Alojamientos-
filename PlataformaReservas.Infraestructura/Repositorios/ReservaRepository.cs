using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;

namespace PlataformaReservas.Infraestructura.Repositorios;

public class ReservaRepository : IReservaRepository
{
    private readonly ApplicationDbContext _context;

    public ReservaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Reserva?> ObtenerPorIdAsync(int id)
    {
        return await _context.Reservas.FindAsync(id);   
    }

    public async Task<bool> TieneReservasLaPropiedadAsync(int propiedadId)
    {
        return await _context.Reservas.AnyAsync(r => r.PropiedadId == propiedadId);
    }

    public async Task AgregarAsync(Reserva reserva)
    {
        await _context.Reservas.AddAsync(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Reserva reserva)
    {
        _context.Reservas.Update(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Reserva>> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.Reservas
            .Where(r => r.UsuarioInvitadoId == usuarioId)
            .ToListAsync();
    }

    public async Task<bool> VerificarDisponibilidadAsync(int propiedadId, DateTime fechaEntrada, DateTime fechaSalida)
    {
        // 1. Buscamos choques con reservas (que no estén canceladas)
        bool existeChoqueReserva = await _context.Reservas
            .AnyAsync(r =>
                r.PropiedadId == propiedadId &&
                r.Estado != Reserva.EstadoEnum.Cancelada &&
                r.FechaEntrada < fechaSalida &&
                r.FechaSalida > fechaEntrada
            );

        // 2. Buscamos choques con bloqueos manuales
        bool existeBloqueoAnfitrion = await _context.FechasBloqueadas
            .AnyAsync(fb =>
                fb.PropiedadId == propiedadId &&
                fb.FechaInicio < fechaSalida &&
                fb.FechaFin > fechaEntrada
            );

        // La propiedad está disponible SOLO si NO hay reserva Y NO hay bloqueo
        // Esto responde a tu duda: Ambas condiciones deben ser falsas para devolver true.
        return !existeChoqueReserva && !existeBloqueoAnfitrion;
    }

    public async Task CrearReservaSeguraAsync(Reserva reserva)
    {
        // IsolationLevel.Serializable garantiza que nadie inserte una reserva 
        // en medio de nuestra validación y el guardado.
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        
        try
        {
            // Ejecutamos ambas comprobaciones dentro de la transacción
            bool existeChoqueReserva = await _context.Reservas
                .AnyAsync(r =>
                    r.PropiedadId == reserva.PropiedadId &&
                    r.Estado != Reserva.EstadoEnum.Cancelada &&
                    r.FechaEntrada < reserva.FechaSalida &&
                    r.FechaSalida > reserva.FechaEntrada
                );

            if (existeChoqueReserva)
            {
                throw new InvalidOperationException("La propiedad ya no está disponible por una reserva confirmada."); 
            }

            bool hayBloqueo = await _context.FechasBloqueadas
                .AnyAsync(fb =>
                    fb.PropiedadId == reserva.PropiedadId &&
                    fb.FechaInicio < reserva.FechaSalida &&
                    fb.FechaFin > reserva.FechaEntrada
                );

            if (hayBloqueo)
            {
                throw new InvalidOperationException("Las fechas seleccionadas han sido bloqueadas por el anfitrión.");
            }

            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }

    public async Task<IEnumerable<Reserva>> ObtenerReservasPorHostIdAsync(int hostId)
    {
        return await _context.Reservas
            .Include(r => r.Propiedad)
            .Include(r => r.UsuarioInvitado) 
            .Where(r => r.Propiedad.HostId == hostId)
            .OrderByDescending(r => r.FechaEntrada)
            .ToListAsync();
    }
}