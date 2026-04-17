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
            var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(hostIdClaim) || !int.TryParse(hostIdClaim, out int hostId))
            {
                return Unauthorized();
            }

            // Ya no hay try/catch. Si esto falla, el Middleware lo atrapa en el aire.
            var fechaBloqueada = await _fechaBloqueadaService.BloquearFechaAsync(dto, hostId);

            return Created("", fechaBloqueada);
        }

        [HttpGet("propiedad/{propiedadId}")] 
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerFechasBloqueadas(int propiedadId)
        {
            var fechas = await _fechaBloqueadaService.ObtenerPorPropiedadAsync(propiedadId);
            return Ok(fechas);
        }
    }
}