using System;
using System.Threading.Tasks;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio.Repositorios;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);


    Task<Usuario?> ObtenerPorIdAsync(int id);


    Task AgregarAsync(Usuario usuario);

    
    Task ActualizarAsync(Usuario usuario);
}

