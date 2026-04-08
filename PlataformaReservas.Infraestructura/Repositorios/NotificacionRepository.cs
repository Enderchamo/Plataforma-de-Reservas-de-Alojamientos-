using System;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;

namespace PlataformaReservas.Infraestructura.Repositorios;

public class NotificacionRepository : INotificacionRepository
{
    private readonly ApplicationDbContext _context;

    public NotificacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ActualizarAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Update(notificacion);
        await _context.SaveChangesAsync();

    }

    public async Task AgregarAsync(Notificacion notificacion)
    {
        await _context.Notificaciones.AddAsync(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioIdAsync(int usuarioId, bool? noLeido = null)
    {
        var query = _context.Notificaciones
            .Where(n => n.UsuarioDestinatarioId == usuarioId);

        if (noLeido.HasValue && noLeido.Value == true)
        {
            query = query.Where(n => n.Leida == false); // Filtramos donde Leida sea falso
        }

        return await query.ToListAsync();
    }
}
