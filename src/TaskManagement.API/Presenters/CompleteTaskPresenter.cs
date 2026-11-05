using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Commands;

namespace TaskManagement.API.Presenters;

/// <summary>
/// <c>CompleteTaskPresenter</c> convierte el resultado del caso de uso de completar una tarea en
/// una respuesta HTTP. Implementa ICompleteTaskOutputBoundary -definido en la capa de aplicación-.
/// </summary>
public sealed class CompleteTaskPresenter : ICompleteTaskOutputBoundary
{
    /// <summary>
    /// La respuesta HTTP que construyó el presenter.
    /// </summary>
    public IActionResult Result { get; private set; } = new StatusCodeResult(StatusCodes.Status500InternalServerError);

    public void PresentSuccess() => Result = new NoContentResult();

    public void PresentNotFound(string message) => Result = ErrorResponses.NotFound(message);

    public void PresentRuleViolation(IReadOnlyList<string> errors) => Result = ErrorResponses.BadRequest(errors);
}
