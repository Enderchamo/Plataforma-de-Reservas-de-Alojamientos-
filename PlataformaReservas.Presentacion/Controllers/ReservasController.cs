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
                var reserva = await _reservaService.CrearReservaAsync(dto);
                
                return Created("", new { mensaje = "Reserva exitosa", id = reserva.Id });
                
        }



        [HttpGet("mis-reservas")]
        
        public async Task<IActionResult> ObtenerMisReservas()

        {

            var reservas = await _reservaService.ObtenerReservasPorUsuarioAsync();
            return Ok(reservas);
            
        }


        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id)
        {

                await _reservaService.CancelarReservaAsync(id);

                return Ok(new {mensaje = "Reserva Cancelada exitosamente."});
        }


        [HttpPatch("{id}/completar")]
        public async Task<IActionResult> CompletarReserva(int id)
        {
        


                await _reservaService.CompletarReservaAsync(id);

                return Ok(new { mensaje = "Reserva marcada como completada." });
        }

    }
}
