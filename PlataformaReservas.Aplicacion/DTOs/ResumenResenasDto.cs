using System.Collections.Generic;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.DTOs;

public class ResumenResenasDto
{
    public double PromedioCalificacion { get; init; }
    public int TotalResenas { get; init; }
    public IEnumerable<Resena> Resenas { get; init; } = new List<Resena>();
}