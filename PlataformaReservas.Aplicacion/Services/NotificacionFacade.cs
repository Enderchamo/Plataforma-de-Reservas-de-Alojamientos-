using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services
{
    public class NotificacionFacade : INotificacionFacade
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificacionFacade> _logger;

        public NotificacionFacade(
            INotificacionRepository notificacionRepository, 
            IEmailService emailService,
            ILogger<NotificacionFacade> logger)
        {
            _notificacionRepository = notificacionRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task EnviarNotificacionYCorreoAsync(int usuarioDestinoId, string correoDestino, string asunto, string mensaje)
        {
            // 1. Guardar en Base de Datos INMEDIATAMENTE (Para que el usuario la vea en la campanita)
            var notificacion = new Notificacion(mensaje, usuarioDestinoId);
            await _notificacionRepository.AgregarAsync(notificacion);

            // 2. Disparar el correo en SEGUNDO PLANO (Fire-and-Forget)
            // C# lanzará esto en otro hilo y la API le responderá al usuario al instante
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.EnviarCorreoAsync(correoDestino, asunto, mensaje); 
                }
                catch (Exception ex)
                {
                    // Si el servidor de correos falla, no tumba la API, solo deja un registro
                    _logger.LogError(ex, "Falló el envío de correo a {Correo}", correoDestino);
                }
            });
        }
    }
}