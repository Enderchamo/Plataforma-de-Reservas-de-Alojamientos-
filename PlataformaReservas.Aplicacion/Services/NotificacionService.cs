using System;
using System.Threading.Tasks;
using System.Linq;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class NotificacionService : INotificacionService
{

    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionService(INotificacionRepository notificacionRepository){

        _notificacionRepository = notificacionRepository;
    }

    public async Task MarcarComoLeidaAsync(int notificacionId, int usuarioId)
    {
        var notificaciones = await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        var notificacion = notificaciones.FirstOrDefault(n => n.Id == notificacionId);

        if (notificacion == null)
        {
            throw new InvalidOperationException("Notificacion no encontrada.");
            
        }

        notificacion.MarcarComoLeida();
        await _notificacionRepository.ActualizarAsync(notificacion);
    }

    public async Task<IEnumerable<Notificacion>> ObtenerMisNotificacionesAsync(int usuarioId, bool? noLeidas = null)
    {
        return await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId,noLeidas);
    }


}
