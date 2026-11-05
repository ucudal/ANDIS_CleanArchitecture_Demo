namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>GetTaskByIdInput</c> es el DTO de entrada del caso de uso de consultar una tarea.
/// </summary>
public sealed record GetTaskByIdInput(Guid TaskId);
