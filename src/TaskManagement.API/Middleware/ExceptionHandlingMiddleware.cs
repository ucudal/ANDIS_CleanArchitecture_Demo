// TaskManagement.API/Middleware/ExceptionHandlingMiddleware.cs
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Middleware;

/// <summary>
/// ExceptionHandlingMiddleware es middleware de ASP.NET Core para manejo centralizado de excepciones.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte de la capa de presentación (UI Layer)</item>
/// <item>Preocupación transversal: Maneja excepciones de todas las capas</item>
/// <item>Pipeline de middleware: Intercepta todas las solicitudes y maneja excepciones</item>
/// <item>Devuelve respuestas HTTP apropiadas para diferentes tipos de error</item>
/// </list>
///
/// Pipeline de Middleware en ASP.NET Core:
/// <list type="bullet">
/// <item>Los componentes de middleware forman un pipeline (como una serie de filtros)</item>
/// <item>Cada middleware puede procesar solicitud y respuesta</item>
/// <item>Los manejadores de excepción generalmente se colocan primero para capturar todas las excepciones</item>
/// <item>Este middleware envuelve otro middleware para capturar excepciones</item>
/// </list>
///
/// Responsabilidades de Manejo de Excepciones:
/// <list type="bullet">
/// <item>Captura excepciones de capa de aplicación y debajo</item>
/// <item>Distingue entre tipos de excepción:
/// <list type="bullet">
/// <item><see cref="ValidationException"/>: 400 BadRequest</item>
/// <item><see cref="NotFoundException"/>: 404 NotFound</item>
/// <item><see cref="DomainException"/>: 400 BadRequest</item>
/// <item>Otras excepciones: 500 InternalServerError</item>
/// </list>
/// </item>
/// <item>Devuelve formato de respuesta de error consistente</item>
/// <item>Registra excepciones para depuración</item>
/// </list>
///
/// Beneficios:
/// <list type="bullet">
/// <item>Manejo de errores centralizado (no disperso en controladores)</item>
/// <item>Formato de respuesta de error consistente en toda la API</item>
/// <item>Previene fuga de detalles de error sensibles a clientes</item>
/// <item>Asegura que todas las excepciones se manejen apropiadamente</item>
/// <item>Simplifica código de controlador (sin necesidad de try-catch)</item>
/// </list>
///
/// Formato de Respuesta de Error:
/// <list type="bullet">
/// <item>ProblemDetails: Formato estándar de respuesta de error</item>
/// <item>Incluye título de error, detalle y código de estado</item>
/// <item>Sigue especificación RFC 7807 Problem Details</item>
/// <item>Ayuda a clientes de API a entender qué salió mal</item>
/// </list>
/// </summary>

internal sealed class ExceptionHandlingMiddleware
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
