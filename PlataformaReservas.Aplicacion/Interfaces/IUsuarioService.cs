using System;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> RegistrarUsuarioAsync(RegistrarUsuarioDto dto);
    
    Task<Usuario> LoginAsync(LoginUsuarioDto dto);

    Task ConfirmarCuentaAsync(string email, string token);

}
