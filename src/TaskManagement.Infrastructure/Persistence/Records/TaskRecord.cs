using TaskManagement.Domain.Entities;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Infrastructure.Persistence.Records;

/// <summary>
/// <c>TaskRecord</c> es el objeto del ORM que representa una fila de la tabla <c>Tasks</c>.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Es lo único que conoce Entity Framework: la entidad TaskItem no sabe que existe un ORM</li>
/// <li>Nunca sale de los adaptadores: TaskRepository lo convierte en TaskItem</li>
/// </ul>
/// </remarks>
public sealed class TaskRecord
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? AssignedTo { get; set; }

    public static TaskRecord FromEntity(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TaskRecord {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            CreatedBy = task.CreatedBy,
            AssignedTo = task.AssignedTo
        };
    }

    public TaskItem ToEntity() => TaskItem.Restore(
        Id,
        Title,
        Description,
        Status,
        Priority,
        DueDate,
        CreatedAt,
        CompletedAt,
        CreatedBy,
        AssignedTo);
}
