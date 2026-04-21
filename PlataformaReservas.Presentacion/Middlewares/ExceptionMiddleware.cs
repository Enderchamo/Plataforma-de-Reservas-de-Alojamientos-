using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PlataformaReservas.Dominio.Excepciones; 

namespace PlataformaReservas.Presentacion.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            if (context.Response.HasStarted)
            {
                logger.LogWarning("No se puede manejar la excepción porque la respuesta ya ha comenzado a enviarse.");
                return Task.CompletedTask;
            }

            context.Response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Ha ocurrido un error inesperado en el servidor.";
            var errorCode = "ERROR_INTERNO";
            IReadOnlyDictionary<string, string[]>? validationErrors = null;

            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                errorCode = appEx.Code;
                logger.LogWarning("Regla de negocio no cumplida: {Message} (Status: {Status})", message, statusCode);
            }
            else if (exception is ValidationException valEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Uno o más campos tienen errores de validación.";
                errorCode = "ERROR_DE_VALIDACION";
                validationErrors = valEx.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());
            }
            else if (exception is InvalidOperationException invEx)
            {
                // Captura el token inválido sin romper la app
                statusCode = (int)HttpStatusCode.BadRequest;
                message = invEx.Message;
                errorCode = "ERROR_DE_DOMINIO";
            }
            else
            {
                logger.LogError(exception, "Ha ocurrido un error crítico no controlado.");
            }

            context.Response.StatusCode = statusCode;

            var error = validationErrors is not null
                ? new ApiError(errorCode, message, validationErrors)
                : new ApiError(errorCode, message);

            var response = new ApiResponse<object>(error);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}