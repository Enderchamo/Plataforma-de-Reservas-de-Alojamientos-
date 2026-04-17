using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadController : ControllerBase
    {
        private readonly IPropiedadService _propiedadService;
        private readonly IPhotoService _photoService;

        public PropiedadController(IPropiedadService propiedadService,IPhotoService photoService)
        {
            _propiedadService = propiedadService;
            _photoService = photoService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPropiedadPorId(int id)
        {
            
                var propiedad = await _propiedadService.ObtenerPorIdAsync(id);
                
                if (propiedad == null)
                {
                    return NotFound(new { error = "Propiedad no encontrada." });
                }

                return Ok(propiedad);
            
        }


        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarPropiedades([FromQuery] string? ubicacion, [FromQuery] decimal? precioMaximo, [FromQuery] int? capacidadMinimas,[FromQuery]DateTime? fechaEntrada, [FromQuery]DateTime? fechaSalida)
        {
            
                var propiedades= await _propiedadService.BuscarPropiedadesAsync(ubicacion,precioMaximo,capacidadMinimas,fechaEntrada,fechaSalida);
                return Ok(propiedades);
            
        }


        [Authorize(Roles = "Host")]
        [HttpPost]
        public async Task<IActionResult> CrearPropiedad([FromBody] CrearPropiedadDto dto)
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

        


        [Authorize(Roles = "Host")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPropiedad(int id, [FromBody] ActualizarPropiedadDto dto)
        {
            // 1. Extraer quién hace la petición
            var hostIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hostIdClaim, out int hostId)) return Unauthorized();

            // 2. Delegar TODO el trabajo y validación al servicio
            await _propiedadService.ActualizarPropiedadAsync(id, dto, hostId);

            // 3. Retornar éxito
            return NoContent();
        }


        [Authorize(Roles = "Host")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPropiedad(int id)
        {
            await _propiedadService.EliminarPropiedadAsync(id);
            return NoContent();
        }


        [Authorize(Roles = "Host")]
        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> SubirImagenPrincipal(int id, IFormFile? imagen) 
        {
            // 1. Validación básica de entrada
            if (imagen == null || imagen.Length == 0)
            {
                return BadRequest(new { error = "Por favor, selecciona una imagen para subir." });
            }

                // 2. Guardamos el archivo físico (Aquí usamos el PhotoService)
                string rutaImagen = await _photoService.GuardarFotoPropiedadAsync(imagen);

                // 3. Llamamos al servicio de propiedad (SIN pasar el hostId)
                // El servicio obtendrá el ID automáticamente desde el IUserContext
                await _propiedadService.ActualizarImagenAsync(id, rutaImagen);

                return Ok(new { 
                    mensaje = "Imagen subida exitosamente",
                    url = rutaImagen 
                });

        }
    }
}
