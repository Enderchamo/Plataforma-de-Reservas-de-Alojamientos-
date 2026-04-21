using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;

namespace PlataformaReservas.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class FechaBloqueadaController : ControllerBase
    {
        private readonly IFechaBloqueadaService _fechaBloqueadaService;

        public FechaBloqueadaController(IFechaBloqueadaService fechaBloqueadaService)
        {
            _fechaBloqueadaService = fechaBloqueadaService;
        }

        [Authorize(Roles = "Host")]
        [HttpPost]
        public async Task<IActionResult> BloquearFecha([FromBody] BloquearFechaDto dto)
        {

            // Ya no hay try/catch. Si esto falla, el Middleware lo atrapa en el aire.
            var fechaBloqueada = await _fechaBloqueadaService.BloquearFechaAsync(dto);

            return Created("", fechaBloqueada);
        }

        [HttpGet("propiedad/{propiedadId}")] 
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerFechasBloqueadas(int propiedadId)
        {
            var fechas = await _fechaBloqueadaService.ObtenerPorPropiedadAsync(propiedadId);
            return Ok(fechas);
        }

        [Authorize(Roles = "Host")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarBloqueo(int id)
        {
            await _fechaBloqueadaService.EliminarBloqueoAsync(id);
            return Ok(new { mensaje = "Bloqueo eliminado correctamente." });
        }
    }
}