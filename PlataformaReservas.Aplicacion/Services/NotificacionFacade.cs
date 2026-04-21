using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Aplicacion.Helpers; 

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
            // 1. Guardar en BD el texto plano (Para que se vea bonito y corto en la campanita de React)
            var notificacion = new Notificacion(mensaje, usuarioDestinoId);
            await _notificacionRepository.AgregarAsync(notificacion);

            // 🛠️ 2. APLICAR LA PLANTILLA AL CORREO
            // Tomamos el mensaje de texto y lo metemos en nuestro diseño HTML de Apex
            string cuerpoHtml = PlantillasEmail.ObtenerPlantillaBase(
                titulo: asunto, 
                mensaje: mensaje, 
                textoBoton: "Ver mis notificaciones", 
                urlBoton: "http://localhost:5173/mis-viajes"
            );

            // 3. Disparar el correo usando el HTML generado
            _ = Task.Run(async () =>
            {
                try
                {
                    // ¡Enviamos cuerpoHtml en vez de mensaje!
                    await _emailService.EnviarCorreoAsync(correoDestino, asunto, cuerpoHtml); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falló el envío de correo a {Correo}", correoDestino);
                }
            });
        }
    }
}