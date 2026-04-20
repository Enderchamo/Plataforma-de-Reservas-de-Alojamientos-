using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> RegistrarUsuarioAsync(RegistrarUsuarioDto dto);
    
    // Cambiamos Usuario por LoginResponseDto para que coincida con tu servicio
    Task<LoginResponseDto> LoginAsync(LoginUsuarioDto dto);

    Task ConfirmarCuentaAsync(string email, string token);

    Task ActualizarRolesAsync( ActualizarRolDto dto);
}