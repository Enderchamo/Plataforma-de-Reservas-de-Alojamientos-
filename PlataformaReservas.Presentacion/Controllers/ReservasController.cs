using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        
        private readonly IReservaService _reservaService;

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }


        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] CrearReservaDto dto)

        {
            
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int guestId))
                
                {
                    return Unauthorized(new { error = "No se pudo identificar al usuario." });
                }

                if (dto.UsuarioInvitadoId != guestId)
                
                {
                    return StatusCode(403, new { error = "No puedes realizar una reserva a nombre de otro usuario." });
                }

                var reserva = await _reservaService.CrearReservaAsync(dto);
                {
                    return Created("", new { mensaje = "Reserva exitosa", id = reserva.Id });
                }
    

        }



        [HttpGet("mis-reservas")]
        
        public async Task<IActionResult> ObtenerMisReservas()

        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int guestId)) 

            {
                return Unauthorized();
            }

            var reservas = await _reservaService.ObtenerReservasPorUsuarioAsync(guestId);
            return Ok(reservas);
            
        }


        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    return Unauthorized();
                }

                await _reservaService.CancelarReservaAsync(id, usuarioId);

                return Ok(new {mensaje = "Reserva Cancelada exitosamente."});
        }


        [HttpPatch("{id}/completar")]
        public async Task<IActionResult> CompletarReserva(int id)
        {
        
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (!int.TryParse(userIdClaim, out int usuarioId)) 
                {
                    return Unauthorized();
                }

                await _reservaService.CompletarReservaAsync(id, usuarioId);

                return Ok(new { mensaje = "Reserva marcada como completada." });
        }

    }
}
