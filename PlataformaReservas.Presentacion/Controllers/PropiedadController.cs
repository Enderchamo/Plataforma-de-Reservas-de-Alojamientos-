using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlataformaReservas.Dominio.Excepciones; // Asegúrate de incluir este using

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadController : ControllerBase
    {
        private readonly IPropiedadService _propiedadService;
        private readonly IPhotoService _photoService;

        public PropiedadController(IPropiedadService propiedadService, IPhotoService photoService)
        {
            _propiedadService = propiedadService;
            _photoService = photoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPropiedadPorId(int id)
        {
            var dto = await _propiedadService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound(new { mensaje = "Propiedad no encontrada" });
            
            return Ok(dto);
        }

        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarPropiedades([FromQuery] string? ubicacion, [FromQuery] decimal? precioMaximo, [FromQuery] int? capacidadMinimas, [FromQuery] DateTime? fechaEntrada, [FromQuery] DateTime? fechaSalida)
        {
            var resultados = await _propiedadService.BuscarPropiedadesAsync(ubicacion, precioMaximo, capacidadMinimas, fechaEntrada, fechaSalida);
            return Ok(resultados);
        }

        [Authorize(Roles = "Host")]
        [HttpPost]
        public async Task<IActionResult> CrearPropiedad([FromBody] CrearPropiedadDto dto)
        {
    
            
            // Pasamos el hostId al servicio para que valide si coincide con el del DTO
            var nuevaPropiedad = await _propiedadService.CrearPropiedadAsync(dto);
            return Created("", nuevaPropiedad);
        }

        [Authorize(Roles = "Host")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPropiedad(int id, [FromBody] ActualizarPropiedadDto dto)
        {
            
            
            // Pasamos el hostId al servicio para que evalúe si el usuario es el dueño
            await _propiedadService.ActualizarPropiedadAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Host")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPropiedad(int id)
        {
            
            
            // Pasamos el hostId al servicio para proteger la eliminación
            await _propiedadService.EliminarPropiedadAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Host")]
        [HttpPost("{id}/imagen")]
        public async Task<IActionResult> SubirImagenPrincipal(int id, IFormFile? imagen) 
        {
            if (imagen == null || imagen.Length == 0)
            {
                // Ahora lanzamos AppException en lugar de devolver BadRequest()
                throw new AppException("Por favor, selecciona una imagen para subir.", 400, "IMAGEN_INVALIDA");
            }

            string rutaImagen = await _photoService.GuardarFotoPropiedadAsync(imagen);
            await _propiedadService.ActualizarImagenAsync(id, rutaImagen);

            return Ok(new { 
                mensaje = "Imagen subida exitosamente",
                url = rutaImagen 
            });
        }

        
    }
}