using System;
namespace PlataformaReservas.Aplicacion.DTOs;

public class CrearPropiedadDto
{
    public string Titulo {get;  init;} = string.Empty;

    public string Ubicacion  {get; init;}  = string.Empty;

    public string Descripcion {get; init;}  = string.Empty;

    public int Capacidad {get; init;}

    public decimal PrecioPorNoche {get; init;}

    public int HostId { get; set; }
}
