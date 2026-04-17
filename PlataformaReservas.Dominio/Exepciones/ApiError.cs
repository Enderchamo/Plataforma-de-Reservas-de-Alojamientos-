using System.Collections.Generic;

namespace PlataformaReservas.Dominio.Excepciones
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; set; }

        public ApiError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public ApiError(string code, string message, IReadOnlyDictionary<string, string[]> validationErrors)
        {
            Code = code;
            Message = message;
            ValidationErrors = validationErrors;
        }
    }
}