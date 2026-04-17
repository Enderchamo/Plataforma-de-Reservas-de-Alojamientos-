using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.Interfaces;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMisNotificaciones([FromQuery] bool? noLeidas)
        {
            // El servicio (gracias a IUserContext) ya sabe quién es el usuario
            var notificaciones = await _notificacionService.ObtenerMisNotificacionesAsync(noLeidas);
            return Ok(notificaciones);
        }

        [HttpPatch("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            // Toda la validación de si la notificación le pertenece al usuario ocurre en el servicio
            await _notificacionService.MarcarComoLeidaAsync(id);
            return Ok(new { mensaje = "Notificación marcada como leída." });
        }
    }
}