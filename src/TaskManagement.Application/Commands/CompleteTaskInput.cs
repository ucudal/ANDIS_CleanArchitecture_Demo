namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CompleteTaskInput</c> es el DTO de entrada del caso de uso de completar una tarea.
/// </summary>
public sealed record CompleteTaskInput(Guid TaskId);
