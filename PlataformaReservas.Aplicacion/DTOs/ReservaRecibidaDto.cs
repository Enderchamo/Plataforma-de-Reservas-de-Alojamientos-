namespace PlataformaReservas.Aplicacion.DTOs;

public class ReservaRecibidaDto
{
    public int Id { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public string Estado { get; set; } = null!;

    // Información simplificada de la propiedad
    public int PropiedadId { get; set; }
    public string PropiedadTitulo { get; set; } = null!;

    // Información simplificada del huésped
    public int HuespedId { get; set; }
    public string HuespedNombre { get; set; } = null!;
    public string HuespedEmail { get; set; } = null!;
}