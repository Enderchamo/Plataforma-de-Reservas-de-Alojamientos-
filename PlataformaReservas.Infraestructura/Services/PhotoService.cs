using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PlataformaReservas.Aplicacion.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace PlataformaReservas.Infraestructura.Services;

public class PhotoService : IPhotoService
{
    private readonly IWebHostEnvironment _env;

    private readonly string[] _extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

    
    public PhotoService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GuardarFotoPropiedadAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            throw new ArgumentException("El archivo está vacío o no es válido.");
        }

        string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        
        if (string.IsNullOrEmpty(extension) || !_extensionesPermitidas.Contains(extension))
        {
            throw new ArgumentException($"Formato no permitido. Solo se aceptan imágenes: {string.Join(", ", _extensionesPermitidas)}");
        }

        if (archivo.Length > 5 * 1024 * 1024)
        {
            throw new ArgumentException("La imagen es demasiado pesada. El tamaño máximo permitido es 5MB.");
        }

    
        string nombreUnico = $"{Guid.NewGuid()}{extension}";

        // ruta física hacia la carpeta  wwwroot/images/propiedadess
        string rutaCarpeta = Path.Combine(_env.WebRootPath, "images", "propiedades");
        
        // Si la carpeta no existe se crea.
        if (!Directory.Exists(rutaCarpeta))
        {
            Directory.CreateDirectory(rutaCarpeta);
        }

        string rutaFisicaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

        // Se copia el archivo a esa ruta física
        using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        // Se retorna la ruta relativa guardada en la Base de Datos
        return $"/images/propiedades/{nombreUnico}";
    }

    public void EliminarFotoPropiedad(string rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa)) 
        
        {
            return;
        }


        string rutaFisica = Path.Combine(_env.WebRootPath, rutaRelativa.TrimStart('/'));

        if (File.Exists(rutaFisica))
        {
            File.Delete(rutaFisica);
        }
    }
}
