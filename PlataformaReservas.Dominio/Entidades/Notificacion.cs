using System;

namespace PlataformaReservas.Dominio.Entidades;

public class Notificacion
{
    public int Id { get; private set; }
    public string? Mensaje { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public bool Leida { get; private set; } 
    public int UsuarioDestinatarioId { get; private set; } 

    public Notificacion(string mensaje, int usuarioDestinatarioId)
    {
        Mensaje = mensaje;
        UsuarioDestinatarioId = usuarioDestinatarioId;
        FechaCreacion = DateTime.UtcNow; 
        Leida = false;
    }

    public void MarcarComoLeida()
    {
        if (!Leida)
        {
            Leida = true;
        }
    }
}