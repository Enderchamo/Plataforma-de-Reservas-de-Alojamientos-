using System;
using System.Threading.Tasks;
using System.Linq;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Dominio.Excepciones;

namespace PlataformaReservas.Aplicacion.Services;

public class NotificacionService : INotificacionService
{

    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUserContext _userContext; 


    public NotificacionService(INotificacionRepository notificacionRepository, IUserContext userContext){

        _notificacionRepository = notificacionRepository;
        _userContext = userContext;
    }

    public async Task MarcarComoLeidaAsync(int notificacionId)
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("No autorizado", 401, "NO_AUTORIZADO");

        var notificaciones = await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        var notificacion = notificaciones.FirstOrDefault(n => n.Id == notificacionId);

        if (notificacion == null)
        {
            throw new AppException("Notificación no encontrada.", 404, "NO_ENCONTRADA");
            
        }

        notificacion.MarcarComoLeida();
        await _notificacionRepository.ActualizarAsync(notificacion);
    }

    public async Task<IEnumerable<Notificacion>> ObtenerMisNotificacionesAsync(bool? noLeidas = null)
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("No autorizado", 401, "NO_AUTORIZADO");
        return await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId,noLeidas);
    }

}
