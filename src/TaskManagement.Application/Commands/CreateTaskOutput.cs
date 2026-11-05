namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CreateTaskOutput</c> es el DTO de salida del caso de uso de crear una tarea.
/// </summary>
public sealed record CreateTaskOutput(Guid TaskId);
