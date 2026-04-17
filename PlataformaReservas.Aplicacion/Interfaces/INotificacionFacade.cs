using System.Threading.Tasks;

namespace PlataformaReservas.Aplicacion.Interfaces
{
    public interface INotificacionFacade
    {
        Task EnviarNotificacionYCorreoAsync(int usuarioDestinoId, string correoDestino, string asunto, string mensaje);
    }
}