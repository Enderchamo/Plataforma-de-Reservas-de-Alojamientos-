using System;

namespace PlataformaReservas.Dominio.Entidades;

public class Propiedad
{
    public int Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descripcion { get; private set; } 
    public string Ubicacion { get; private set; }   
    public decimal PrecioPorNoche { get; private set; }
    public int Capacidad { get; private set; }      
    public int HostId { get; private set; }
    public string? ImagenUrl { get; set; }
    public byte[]? Version { get; private set; }

    public Propiedad(string titulo, string descripcion, string ubicacion, decimal precioPorNoche, int capacidad, int hostId)
    {
        if (precioPorNoche <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");
            
        if (capacidad <= 0)
            throw new ArgumentException("La capacidad debe ser de al menos 1 persona.");

        Titulo = titulo;
        Descripcion = descripcion;
        Ubicacion = ubicacion;
        PrecioPorNoche = precioPorNoche;
        Capacidad = capacidad;
        HostId = hostId;
    }


    public void ActualizarDetalles(string titulo, string descripcion, string ubicacion, decimal precioPorNoche, int capacidad)
    {
        if (precioPorNoche <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");
        
        if (capacidad <= 0)
            throw new ArgumentException("La capacidad debe ser de al menos 1 persona.");

        Titulo = titulo;
        Descripcion = descripcion;
        Ubicacion = ubicacion;
        PrecioPorNoche = precioPorNoche;
        Capacidad = capacidad;
    }
}