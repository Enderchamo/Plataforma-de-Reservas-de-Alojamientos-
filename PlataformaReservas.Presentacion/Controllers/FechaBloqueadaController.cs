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
            try
            {
                var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(hostIdClaim) || !int.TryParse(hostIdClaim, out int hostId))

                {
                    return Unauthorized();
                }

                var fechaBloqueada = await _fechaBloqueadaService.BloquearFechaAsync(dto, hostId);

                return Created("", fechaBloqueada);
            }

            catch (FluentValidation.ValidationException ex)

            {
                return BadRequest(new { errores = ex.Errors.Select(e => e.ErrorMessage) });
            }

            catch (InvalidOperationException ex)

            {
                return NotFound(new { error = ex.Message });
            }

            catch (UnauthorizedAccessException ex)

            {
                return StatusCode(403, new { error = ex.Message });
            }

            catch (ArgumentException ex) 

            {
                return BadRequest(new { error = ex.Message });
            }

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
