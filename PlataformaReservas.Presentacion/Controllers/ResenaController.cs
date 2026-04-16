using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenaController : ControllerBase
    {
        private readonly IResenaService _resenaService;

        public ResenaController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        [Authorize(Roles = "Guest")] 
        [HttpPost]
        public async Task<IActionResult> CrearResena([FromBody] CrearResenaDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int usuarioId)) 
                {
                    return Unauthorized();
                }

                var resena = await _resenaService.CrearResenaAsync(dto, usuarioId);

                return Created("", resena);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errores = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (System.InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (System.UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
        }
        
        [HttpGet("propiedad/{propiedadId}")]
        public async Task<IActionResult> ObtenerResenasPorPropiedad(int propiedadId)
        {
            var resumen = await _resenaService.ObtenerResumenPorPropiedadAsync(propiedadId);
            return Ok(resumen);
        }
    }
}