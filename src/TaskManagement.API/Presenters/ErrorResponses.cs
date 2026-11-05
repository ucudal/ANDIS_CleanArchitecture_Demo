using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Presenters;

/// <summary>
/// <c>ErrorResponses</c> construye las respuestas de error que comparten los presenters.
/// </summary>
internal static class ErrorResponses
{
    public static IActionResult NotFound(string message) => new NotFoundObjectResult(new ProblemDetails {
        Title = "Request failed",
        Detail = message,
        Status = StatusCodes.Status404NotFound
    });

    public static IActionResult BadRequest(IReadOnlyList<string> errors) => new BadRequestObjectResult(new ProblemDetails {
        Title = "Request failed",
        Detail = string.Join("; ", errors),
        Status = StatusCodes.Status400BadRequest
    });

    public static IActionResult ValidationFailed(IReadOnlyList<string> errors) => new BadRequestObjectResult(
        new ValidationProblemDetails(new Dictionary<string, string[]> { ["General"] = [.. errors] }) {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        });
}
