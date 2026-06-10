// TaskManagement.API/Middleware/ExceptionHandlingMiddleware.cs
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Middleware;

/// <summary>
/// <c>ExceptionHandlingMiddleware</c> es <a
/// href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0">middleware</a>
/// de ASP.NET Core para manejo centralizado de excepciones.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, %API Layer</li>
/// <li>Preocupación transversal: maneja excepciones de todas las capas</li>
/// <li>Pipeline de middleware: intercepta todas las solicitudes y maneja excepciones</li>
/// <li>Devuelve respuestas HTTP apropiadas para diferentes tipos de error</li>
/// </ul>
///
/// Pipeline de middleware en ASP.NET Core:
/// <ul>
/// <li>Los componentes de middleware forman un pipeline</li>
/// <li>Cada middleware puede procesar solicitud y respuesta</li>
/// <li>Los manejadores de excepción generalmente se colocan primero para
/// capturar todas las excepciones</li>
/// <li>Este middleware envuelve otro middleware para capturar excepciones</li>
/// </ul>
///
/// Responsabilidades de manejo de excepciones:
/// <ul>
/// <li>Captura excepciones de capa de aplicación y dominio</li>
/// <li>Distingue entre tipos de excepción:
/// <ul>
/// <li>ValidationException: 400 BadRequest</li>
/// <li>NotFoundException: 404 NotFound</li>
/// <li>DomainException: 400 BadRequest</li>
/// <li>Otras excepciones: 500 InternalServerError</li>
/// </ul>
/// </li>
/// <li>Devuelve formato de respuesta de error consistente</li>
/// <li>Registra excepciones para depuración</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Manejo de errores centralizado, no queda disperso en controladores</li>
/// <li>Formato de respuesta de error consistente en toda la %API</li>
/// <li>Previene fuga de detalles de error sensibles a clientes</li>
/// <li>Asegura que todas las excepciones se manejen apropiadamente</li>
/// <li>Simplifica código de controlador pues no hay necesidad de try-catch</li>
/// </ul>
///
/// Formato de respuesta de error:
/// <ul>
/// <li>ProblemDetails: formato estándar de respuesta de error</li>
/// <li>Incluye título de error, detalle y código de estado</li>
/// <li>Sigue especificación RFC 7807 Problem Details</li>
/// <li>Ayuda a clientes de la API a entender qué salió mal</li>
/// </ul>
/// </remarks>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly Action<ILogger, string, Exception?> LogRequestFailure =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1000, nameof(ExceptionHandlingMiddleware)),
            "Request failed: {Message}");

    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context).ConfigureAwait(false);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            LogRequestFailure(_logger, ex.Message, ex);
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                (ProblemDetails)new ValidationProblemDetails(ve.Errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                }),
            NotFoundException ne => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Title = "Resource not found",
                    Detail = ne.Message,
                    Status = StatusCodes.Status404NotFound
                }),
            DomainException de => (
                StatusCodes.Status422UnprocessableEntity,
                new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = de.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                }),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Please try again later or contact support."
                })
        };
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
