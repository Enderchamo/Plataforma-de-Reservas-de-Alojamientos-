using System;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;


namespace PlataformaReservas.Infraestructura.Repositorios;

public class ResenaRepository : IResenaRepository
{   
    private readonly ApplicationDbContext _context;

    public ResenaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task AgregarAsync(Resena resena)
    {
        await _context.Resenas.AddAsync(resena);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Resena>> ObtenerPorPropiedadIdAsync(int propiedadId)
    {
        var query = from resena in _context.Resenas
                    join reserva in _context.Reservas on resena.ReservaId equals reserva.Id
                    where reserva.PropiedadId == propiedadId
                    select resena;
        return await query.ToListAsync();
    }

    public async Task<bool> ExisteResenaPorReservaAsync(int reservaId)
    {
        return await _context.Resenas.AnyAsync(r => r.ReservaId == reservaId);
    }
}
