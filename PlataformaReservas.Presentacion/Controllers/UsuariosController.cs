using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _config;
        public UsuariosController(IUsuarioService usuarioService, IConfiguration config)
        {
            _usuarioService = usuarioService;
            _config = config;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
        {
            try
            {
                var usuario = await _usuarioService.RegistrarUsuarioAsync(dto);

                return Created("", new
                {
                    mensaje="Usuario registrado exitosamente. Revisa tu correo para el token de confirmación.",
                    usuarioId=usuario.Id,
                    email = usuario.Email,
                    tokenConfirmacion= usuario.TokenConfirmacion
                });
            }

            catch (FluentValidation.ValidationException ex)
            {    
                return BadRequest(new { errores = ex.Errors.Select(e => e.ErrorMessage)});
            }

            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        [HttpPost("confirmar")]
        public async Task<IActionResult> ConfirmarCuenta([FromQuery] string email,[FromQuery] string token)
        {
            try
            {
                await _usuarioService.ConfirmarCuentaAsync(email, token);
                return Ok(new{ mensaje = "Cuenta confirmada exitosamente. Ya puedes iniciar sesion. "});              
            }
            catch (Exception ex)
            {
                
                return BadRequest(new {error = ex.Message});
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(dto);
                

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _config["Jwt:Key"] ?? "NoTeMolestesEnIntentarAdivinarEstSUperClave1234Pollopica";
                var key = Encoding.UTF8.GetBytes(jwtKey);
                
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email)
                };

                
                if (usuario.EsHost) claims.Add(new Claim(ClaimTypes.Role, "Host"));
                if (usuario.EsGuest) claims.Add(new Claim(ClaimTypes.Role, "Guest"));

            
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1), 
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                
            
                var token = tokenHandler.CreateToken(tokenDescriptor);
                
                return Ok(new { 
                    mensaje = "Login exitoso",
                    token = tokenHandler.WriteToken(token) 
                });
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errores = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            }
        }
}
