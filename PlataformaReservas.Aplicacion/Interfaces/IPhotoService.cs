using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IPhotoService
{
    // Recibe el archivo, lo guarda y devuelve la ruta relativa .
    Task<string> GuardarFotoPropiedadAsync(IFormFile archivo);
    
    //para borrar la foto si se elimina la propiedad
    void EliminarFotoPropiedad(string rutaRelativa);
}
