using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Aplicacion.Services;
using System.Security.Claims;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenaController : ControllerBase
    {
        public readonly IResenaService _resenaService;

        public ResenaController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        [Authorize(Roles = "Guest")] //Solo huéspedes pueden evaluar
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

            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }

            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
        }
        
        [HttpGet("propiedad/{propiedadId}")]
        public async Task<IActionResult> ObtenerResenasPorPropiedad(int propiedadId)
        {
            var resenas = await _resenaService.ObtenerPorPropiedadAsync(propiedadId);
            
            // Calculo del promedio (Ahorrar trabajo en el Front-end)
            double promedio = resenas.Any() ? resenas.Average(r => r.Calificacion) : 0;

            return Ok(new 
            { 
                promedioCalificacion = Math.Round(promedio, 1),
                totalResenas = resenas.Count(),
                resenas = resenas 
            });
            
        }

    }
}
