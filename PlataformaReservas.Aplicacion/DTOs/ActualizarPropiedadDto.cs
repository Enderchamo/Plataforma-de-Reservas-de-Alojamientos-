namespace PlataformaReservas.Aplicacion.DTOs
{
    public class ActualizarPropiedadDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioPorNoche { get; set; }
        public int Capacidad { get; set; }

        public string Ubicacion { get;  set; } = string.Empty;
        
    }
}