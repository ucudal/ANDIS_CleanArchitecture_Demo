using TaskManagement.Domain.Common;
using TaskManagement.Domain.Events;

namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskItem</c> es una <a
/// href="https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Entity.md">entidad</a>
/// del dominio que representa una tarea en el sistema.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa del dominio</li>
/// <li>Encapsula lógica de negocio y reglas relacionadas con la gestión de
/// tareas</li>
/// <li>Contiene principios de %Domain Driven Design con el patrón <a
/// href="https://martinfowler.com/bliki/DDD_Aggregate.html">Aggregate
/// Root</a></li>
/// <li>Gestiona transiciones de estado a través de métodos del dominio:
/// TaskItem.Complete, TaskItem.AssignTo, TaskItem.UpdatePriority</li>
/// <li>Mantiene invariantes y reglas de negocio tales como validación y
/// restricciones</li>
/// <li>Emite eventos del dominio para comunicar ocurrencias importantes del
/// dominio</li>
/// </ul>
///
/// Características clave:
/// <ul>
/// <li>Contiene todos los datos necesarios para representar una tarea</li>
/// <li>Valida reglas de negocio internamente -longitud de título, fecha de
/// vencimiento, transiciones de estado-</li>
/// <li>Gestiona la colección de eventos del dominio para desacoplamiento de la
/// infraestructura y facilidad del testing</li>
/// <li>Utiliza el patrón <a
/// href="https://refactoring.guru/design-patterns/factory-method">Factory</a>
/// -verTaskItem.Create- para creación consistente de entidades</li>
/// <li>Aplica restricciones de negocio: no se puede modificar tareas
/// completadas, etc.</li>
/// </ul>
///
/// Dependencias:
/// <ul>
/// <li>Solo depende de otros tipos de la capa del dominio -verDomainEvent,
/// Result, TaskErrors, etc.-</li>
/// <li>Sin dependencias en capas de infraestructura o aplicación -respetala
/// dirección de las dependencias en Clean Architecture-</li>
/// </ul>
/// </remarks>
public class TaskItem
{
    public Guid Id {
        get; private set;
    }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskStatus Status {
        get; private set;
    }

    public TaskPriority Priority {
        get; private set;
    }

    public DateTime? DueDate {
        get; private set;
    }

    public DateTime CreatedAt {
        get; private set;
    }

    public DateTime? CompletedAt {
        get; private set;
    }

    public Guid CreatedBy {
        get; private set;
    }

    public Guid? AssignedTo {
        get; private set;
    }

    // Eventos del dominio para desacoplamiento de la infraestructura y facilidad del testing.
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Constructor protegido de EF Core
    protected TaskItem()
    {
    }

    public static Result<TaskItem> Create(
        string title,
        string description,
        TaskPriority priority,
        DateTime? dueDate,
        Guid createdBy)
    {
        ArgumentNullException.ThrowIfNull(description);

        var validation = Validate(title, description, dueDate);
        if (validation.IsFailure)
            return Result.Failure<TaskItem>(validation.Errors);
        var task = new TaskItem {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            Status = TaskStatus.Todo,
            Priority = priority,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        task.AddDomainEvent(new TaskCreatedEvent(task.Id, task.Title, createdBy));
        return Result.Success(task);
    }
    public Result AssignTo(Guid userId)
    {
        if (Status == TaskStatus.Completed)
            return Result.Failure(TaskErrors.CannotAssignCompletedTask);
        var previousAssignee = AssignedTo;
        AssignedTo = userId;
        AddDomainEvent(new TaskAssignedEvent(Id, userId, previousAssignee));
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status == TaskStatus.Completed)
            return Result.Failure(TaskErrors.AlreadyCompleted);
        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new TaskCompletedEvent(Id, CompletedAt.Value));
        return Result.Success();
    }

    public Result UpdatePriority(TaskPriority newPriority)
    {
        if (Status == TaskStatus.Completed)
            return Result.Failure(TaskErrors.CannotModifyCompletedTask);
        var oldPriority = Priority;
        Priority = newPriority;
        AddDomainEvent(new TaskPriorityChangedEvent(Id, oldPriority, newPriority));
        return Result.Success();
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void AddDomainEvent(DomainEvent eventItem) => _domainEvents.Add(eventItem);

    private static Result Validate(string title, string description, DateTime? dueDate)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(title))
            errors.Add("Title is required.");
        else if (title.Length > 200)
            errors.Add("Title cannot exceed 200 characters.");
        if (description.Length > 2000)
            errors.Add("Description cannot exceed 2000 characters.");
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow.Date)
            errors.Add("Due date cannot be in the past.");
        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }
}
