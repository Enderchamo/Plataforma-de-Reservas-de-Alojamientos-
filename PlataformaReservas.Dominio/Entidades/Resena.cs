using System;

namespace PlataformaReservas.Dominio.Entidades;

public class Resena
{
    public int Id{get;private set;}
    public int ReservaId{get;private set;}

    public int Calificacion{get;private set;}

    public string Comentario{get;private set;}


    public Resena(int reservaId, int calificacion, string comentario)
    {
        ReservaId = reservaId;

        if (calificacion <= 0 || calificacion >5)
        {
            throw new ArgumentException("La calificación debe estar entre 1 y 5.");
        }
        Comentario = comentario;

        Calificacion = calificacion;
    }
}
