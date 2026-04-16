using Microsoft.AspNetCore.Http; // Importante también aquí

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IPhotoService
{
    Task<string> GuardarFotoPropiedadAsync(IFormFile archivo);
}