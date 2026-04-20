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

    public async Task AgregarAsync( Reserva reserva)
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
        
        bool existeChoque = await _context.Reservas
            .AnyAsync(r =>

                r.PropiedadId == propiedadId &&
                r.Estado != Reserva.EstadoEnum.Cancelada &&
                r.FechaEntrada < fechaSalida &&
                r.FechaSalida > fechaEntrada

            );

            return !existeChoque;
    }

    public async Task CrearReservaSeguraAsync(Reserva reserva)
    {
    
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        
        try
        {
        
            bool existeChoque = await _context.Reservas
                .AnyAsync(r =>
                    r.PropiedadId == reserva.PropiedadId &&
                    r.Estado != Reserva.EstadoEnum.Cancelada &&
                    r.FechaEntrada < reserva.FechaSalida &&
                    r.FechaSalida > reserva.FechaEntrada
                );

            if (existeChoque)
            {
                
                throw new InvalidOperationException("La propiedad ya no está disponible."); 
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
        // Incluimos la Propiedad para poder filtrar por su HostId y ver sus datos
        .Include(r => r.Propiedad)
        // Incluimos al UsuarioInvitado para que el Host sepa quién le reservó
        .Include(r => r.UsuarioInvitado) 
        // Filtramos: Solo las reservas donde el dueño de la propiedad sea el Host actual
        .Where(r => r.Propiedad.HostId == hostId)
        // Ordenamos para que las reservas más recientes (por fecha de entrada) salgan primero
        .OrderByDescending(r => r.FechaEntrada)
        .ToListAsync();
    }

}
