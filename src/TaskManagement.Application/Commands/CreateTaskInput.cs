using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CreateTaskInput</c> es el DTO de entrada del caso de uso de crear una tarea.
/// </summary>
public sealed record CreateTaskInput(
    string Title,
    string Description,
    TaskPriorityLevel Priority,
    DateTime? DueDate,
    Guid CreatedBy);
