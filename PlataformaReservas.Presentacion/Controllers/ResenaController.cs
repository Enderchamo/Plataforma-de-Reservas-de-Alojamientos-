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
            
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int usuarioId)) 
                {
                    return Unauthorized();
                }

                var resena = await _resenaService.CrearResenaAsync(dto, usuarioId);

                return Created("", resena);
    
        }
        
        [HttpGet("propiedad/{propiedadId}")]
        public async Task<IActionResult> ObtenerResenasPorPropiedad(int propiedadId)
        {
            var resumen = await _resenaService.ObtenerResumenPorPropiedadAsync(propiedadId);
            return Ok(resumen);
        }
    }
}