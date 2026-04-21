namespace PlataformaReservas.Aplicacion.DTOs;

public class PropiedadListaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public decimal PrecioPorNoche { get; set; }
    public string Ubicacion { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public int Capacidad { get; set; }
    public int HostId { get; set; }
}