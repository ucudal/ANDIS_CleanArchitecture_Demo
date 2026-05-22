using TaskManagement.Domain.Common;
using TaskManagement.Domain.Events;

namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskItem</c> es una <a
/// href="https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Entity.md">entidad</a>
/// de dominio que representa una tarea en el sistema.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte del core de la aplicación en la capa de dominio</item>
/// <item>Encapsula lógica de negocio y reglas relacionadas con la gestión de
/// tareas</item>
/// <item>Contiene principios de Domain Driven Design con el patrón <a
/// href="https://martinfowler.com/bliki/DDD_Aggregate.html">Aggregate
/// Root</a></item>
/// <item>Gestiona transiciones de estado a través de métodos de dominio: <see
/// cref="TaskItem.Complete"/>, <see cref="TaskItem.AssignTo"/>, <see
/// cref="TaskItem.UpdatePriority"/></item>
/// <item>Mantiene invariantes y reglas de negocio tales como validación y
/// restricciones</item>
/// <item>Emite eventos de dominio para comunicar ocurrencias importantes del
/// dominio</item>
/// </list>
///
/// Características clave:
/// <list type="bullet">
/// <item>Contiene todos los datos necesarios para representar una tarea</item>
/// <item>Valida reglas de negocio internamente -longitud de título, fecha de
/// vencimiento, transiciones de estado-</item>
/// <item>Gestiona la colección de eventos de dominio para desacoplamiento de la
/// infraestructura y facilidad del testing</item>
/// <item>Utiliza patrón <a
/// href="https://refactoring.guru/design-patterns/factory-method">Factory</a>
/// <see cref="TaskItem.Create(string, string, TaskPriority, DateTime?, Guid)"/>
/// para creación consistente de entidades</item>
/// <item>Aplica restricciones de negocio: no se puede modificar tareas
/// completadas, etc.</item>
/// </list>
///
/// Dependencias: Solo depende de otros tipos de la capa de Dominio —<see
/// cref="DomainEvent"/>, <see cref="Result"/>, <see cref="TaskErrors"/>) Sin
/// dependencias en capas de Infraestructura o Aplicación - mantiene
/// independencia para testabilidad.
/// </summary>
public class TaskItem
{
    public Guid Id
    {
        get; private set;
    }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskStatus Status
    {
        get; private set;
    }

    public TaskPriority Priority
    {
        get; private set;
    }

    public DateTime? DueDate
    {
        get; private set;
    }

    public DateTime CreatedAt
    {
        get; private set;
    }

    public DateTime? CompletedAt
    {
        get; private set;
    }

    public Guid CreatedBy
    {
        get; private set;
    }

    public Guid? AssignedTo
    {
        get; private set;
    }

    // Eventos de dominio para desacoplamiento de la infraestructura y facilidad del testing.
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
        var task = new TaskItem
        {
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
