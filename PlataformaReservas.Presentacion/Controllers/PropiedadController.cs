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
    public class PropiedadController : ControllerBase
    {
        private readonly IPropiedadService _propiedadService;

        public PropiedadController(IPropiedadService propiedadService)
        {
            _propiedadService = propiedadService;
        }

        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarPropiedades([FromQuery] string? ubicacion, [FromQuery] decimal? precioMaximo, [FromQuery] int? capacidadMinimas,[FromQuery]DateTime? fechaEntrada, [FromQuery]DateTime? fechaSalida)
        {
            try
            {
                var propiedades= await _propiedadService.BuscarPropiedadesAsync(ubicacion,precioMaximo,capacidadMinimas,fechaEntrada,fechaSalida);
                return Ok(propiedades);
            }
            catch (ArgumentException ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
        }


        [Authorize(Roles = "Host")]
        [HttpPost]
        public async Task<IActionResult> CrearPropiedad([FromBody] CrearPropiedadDto dto)
        {
            
            try
            {
                var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(hostIdClaim ) || !int.TryParse(hostIdClaim,out int hostId))
                {
                    return Unauthorized(new {error = "No se puedo identificar al usuario."});  
                }

                if (dto.HostId != hostId)
                {
                    return StatusCode(403, new {error = "No tienes permiso para crear propiedades a nombre de otro host."});
                }

                var nuevaPropiedad = await _propiedadService.CrearPropiedadAsync(dto);
                return Created("",nuevaPropiedad);
            }

            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }

            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errores = ex.Errors.Select(e => e.ErrorMessage) });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor." });
            }

        }

        


        [Authorize(Roles = "Host")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPropiedad(int id, [FromBody] CrearPropiedadDto dto)
        {
            try
            {
                var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(hostIdClaim) || !int.TryParse(hostIdClaim, out int hostId))  
                {
                    return Unauthorized();
                }

                await _propiedadService.ActualizarPropiedadAsync(id,dto,hostId);
                return NoContent();
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


        }


        [Authorize(Roles = "Host")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPropiedad(int id)
        {
            try
            {
                var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(hostIdClaim) || !int.TryParse(hostIdClaim, out int hostId))
                    return Unauthorized();

                await _propiedadService.EliminarPropiedadAsync(id, hostId);
                return NoContent(); // 204 No Content (Eliminación exitosa)
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message }); 
            }
        }


    }
}
