namespace TaskManagement.API.Requests;

using TaskManagement.Domain.Entities;

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate
);
