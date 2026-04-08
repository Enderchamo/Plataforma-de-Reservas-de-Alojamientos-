using System;
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

}
