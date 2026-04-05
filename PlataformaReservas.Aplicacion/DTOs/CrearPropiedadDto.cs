using System;
namespace PlataformaReservas.Aplicacion.DTOs;

public class CrearPropiedadDto
{
    public string Titulo {get;  init;}

    public string Ubicacion  {get; init;}

    public string Descripcion {get; init;}

    public int Capacidad {get; init;}

    public decimal PrecioPorNoche {get; init;}

    public int HostId {get; init;}


}
