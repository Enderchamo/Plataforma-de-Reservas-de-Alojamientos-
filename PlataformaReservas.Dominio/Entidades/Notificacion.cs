using System;

namespace PlataformaReservas.Dominio.Entidades;

public class Notificacion
{
    public int Id { get; private set;}

    public string? Mensaje { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    public bool leida { get; private set; }

    public int UsuarioDestinatario { get; private set; }

    public Notificacion(string mensaje, int usuarioDestinatario)
    {
        Mensaje = mensaje;
        UsuarioDestinatario = usuarioDestinatario;
        FechaCreacion = DateTime.Now;
        leida = false;
    }

    public void MarcarComoLeido()
    {
        if (!this.leida)
        {
            this.leida = true;
        }
    }


}
