using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.ViewModels;
using TaskManagement.Application.Queries;

namespace TaskManagement.API.Presenters;

/// <summary>
/// <c>GetTaskByIdPresenter</c> convierte el resultado del caso de uso de consultar una tarea en una
/// respuesta HTTP. Implementa IGetTaskByIdOutputBoundary -definido en la capa de aplicación- y
/// construye el TaskViewModel a partir del TaskDto.
/// </summary>
public sealed class GetTaskByIdPresenter : IGetTaskByIdOutputBoundary
{
    /// <summary>
    /// La respuesta HTTP que construyó el presenter.
    /// </summary>
    public IActionResult Result { get; private set; } = new StatusCodeResult(StatusCodes.Status500InternalServerError);

    public void PresentSuccess(TaskDto task)
    {
        ArgumentNullException.ThrowIfNull(task);

        Result = new OkObjectResult(new TaskViewModel {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CreatedBy = task.CreatedBy,
            AssignedTo = task.AssignedTo
        });
    }

    public void PresentNotFound(string message) => Result = ErrorResponses.NotFound(message);
}
