using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Requests;

internal sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate
);
