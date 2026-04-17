namespace PlataformaReservas.Dominio.Excepciones
{
    public class ApiResponse<T>
    {
        public bool Ok { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }

        // Constructor para respuestas exitosas
        public ApiResponse(T data)
        {
            Ok = true;
            Data = data;
        }

        // Constructor para respuestas con error
        public ApiResponse(ApiError error)
        {
            Ok = false;
            Error = error;
        }
    }
}