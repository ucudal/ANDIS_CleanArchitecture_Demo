// TaskManagement.API/Middleware/ExceptionHandlingMiddleware.cs
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Middleware;

/// <summary>
/// <c>ExceptionHandlingMiddleware</c> convierte las excepciones inesperadas en una respuesta HTTP
/// 500.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, una %API REST</li>
/// <li>Es la última línea de defensa para fallas técnicas -base de datos caída, errores de
/// programación-</li>
/// <li>Evita que los detalles internos de una falla lleguen al cliente</li>
/// </ul>
///
/// Las violaciones de reglas de negocio y las tareas inexistentes no son excepciones: los
/// interactores las entregan por su output boundary y las presentan los presenters.
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
            await WriteErrorResponseAsync(context).ConfigureAwait(false);
        }
    }

    private static Task WriteErrorResponseAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsJsonAsync(new ProblemDetails {
            Title = "An unexpected error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Please try again later or contact support."
        });
    }
}
