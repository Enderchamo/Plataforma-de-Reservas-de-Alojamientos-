using System;
using System.Threading.Tasks;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface INotificacionService
{
    Task<IEnumerable<Notificacion>> ObtenerMisNotificacionesAsync(bool? noLeidas = null);
    Task MarcarComoLeidaAsync(int notificacionId);
}
