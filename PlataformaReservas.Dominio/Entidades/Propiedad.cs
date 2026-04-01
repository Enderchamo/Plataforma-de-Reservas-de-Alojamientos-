using System;

namespace PlataformaReservas.Dominio.Entidades;

public class Propiedad
{
    public string Titulo {get; private set;}

    public string Direccion {get; private set;}

    public decimal PrecioPorNoche {get; private set;}

    public int HostId {get; private set;}

    public Propiedad(string titulo, string direccion, decimal precioPorNoche, int hostId)
    {
        Titulo=titulo;
        Direccion= direccion;
        
        if (precioPorNoche <= 0)
        {
            throw new ArgumentException ("Precio debe ser mayor a 0");
        }

        PrecioPorNoche= precioPorNoche;
        HostId= hostId;
    }
}
