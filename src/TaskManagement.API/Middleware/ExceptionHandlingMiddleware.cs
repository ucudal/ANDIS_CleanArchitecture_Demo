// TaskManagement.API/Middleware/ExceptionHandlingMiddleware.cs
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Middleware;

/// <summary>
/// ExceptionHandlingMiddleware is ASP.NET Core middleware for centralized exception handling.
///
/// Role in Clean Architecture:
/// - Part of the UI Layer (API presentation)
/// - Cross-cutting concern: Handles exceptions from all layers
/// - Middleware pipeline: Intercepts all requests and handles exceptions
/// - Returns appropriate HTTP responses for different error types
///
/// Middleware Pipeline in ASP.NET Core:
/// - Middleware components form a pipeline (like a series of filters)
/// - Each middleware can process request and response
/// - Exception handlers are usually placed early to catch all exceptions
/// - This middleware wraps other middleware to catch exceptions
///
/// Exception Handling Responsibilities:
/// - Catch exceptions from application layer and below
/// - Distinguish between exception types:
///   - ValidationException: 400 BadRequest
///   - NotFoundException: 404 NotFound
///   - DomainException: 400 BadRequest
///   - Other exceptions: 500 InternalServerError
/// - Return consistent error response format
/// - Log exceptions for debugging
///
/// Benefits:
/// - Centralized error handling (not scattered in controllers)
/// - Consistent error response format across API
/// - Prevents leaking sensitive error details to clients
/// - Ensures all exceptions are handled appropriately
/// - Simplifies controller code (no try-catch needed)
///
/// Error Response Format:
/// - ProblemDetails: Standard error response format
/// - Includes error title, detail, and status code
/// - Follows RFC 7807 Problem Details specification
/// - Helps API clients understand what went wrong
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
