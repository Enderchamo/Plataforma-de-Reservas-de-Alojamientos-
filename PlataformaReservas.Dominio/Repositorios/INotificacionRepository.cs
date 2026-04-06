using System;
using PlataformaReservas.Dominio.Entidades;




namespace PlataformaReservas.Dominio.Repositorios;

public interface INotificacionRepository
{
    Task AgregarAsync(Notificacion notificacion );
    Task ActualizarAsync(Notificacion notificacion );

    Task<IEnumerable<Notificacion>> ObtenerPorUsuarioIdAsync(int usuarioId, bool? noLeido=null);
}
