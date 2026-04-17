using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        public readonly INotificacionService _notificacionService;


        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMisNotificaciones([FromQuery] bool? noLeidas)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            var notificaciones = await _notificacionService.ObtenerMisNotificacionesAsync(usuarioId, noLeidas);
            
            return Ok(notificaciones);
        }

        [HttpPatch("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int usuarioId)) 
                {
                    return Unauthorized();
                }

                await _notificacionService.MarcarComoLeidaAsync(id, usuarioId);

                return Ok(new { mensaje = "Notificación marcada como leída." });
            
        }
    }
}
