using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Controllers;
using TaskManagement.Application.Commands;

namespace TaskManagement.API.Presenters;

/// <summary>
/// <c>CreateTaskPresenter</c> convierte el resultado del caso de uso de crear una tarea en una
/// respuesta HTTP.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, una %API REST</li>
/// <li>Implementa ICreateTaskOutputBoundary -definido en la capa de aplicación-</li>
/// <li>El caso de uso lo llama sin conocerlo; el controller lee Result al terminar</li>
/// <li>Decide los códigos de estado y el formato de la respuesta: el caso de uso no sabe de HTTP</li>
/// <li>Hay una instancia por solicitud: es el mismo objeto para el caso de uso y el controller</li>
/// </ul>
/// </remarks>
public sealed class CreateTaskPresenter : ICreateTaskOutputBoundary
{
    /// <summary>
    /// La respuesta HTTP que construyó el presenter.
    /// </summary>
    public IActionResult Result { get; private set; } = new StatusCodeResult(StatusCodes.Status500InternalServerError);

    public void PresentSuccess(CreateTaskOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        Result = new CreatedAtActionResult(
            nameof(TasksController.GetById),
            "Tasks",
            new {
                id = output.TaskId
            },
            output.TaskId);
    }

    public void PresentValidationErrors(IReadOnlyList<string> errors) =>
        Result = ErrorResponses.ValidationFailed(errors);
}
