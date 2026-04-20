namespace PlataformaReservas.Aplicacion.DTOs;

public class PropiedadDetalleDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string Ubicacion { get; set; } = null!;
    public decimal PrecioPorNoche { get; set; }
    public int Capacidad { get; set; }
    public string? ImagenUrl { get; set; }
    public int HostId { get; set; }
    public string NombreHost { get; set; } = null!;
    
}