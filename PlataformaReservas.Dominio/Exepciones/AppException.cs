using System;

namespace PlataformaReservas.Dominio.Excepciones
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string Code { get; }

        public AppException(string message, int statusCode = 400, string code = "ERROR_DE_NEGOCIO") 
            : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }
    }
}