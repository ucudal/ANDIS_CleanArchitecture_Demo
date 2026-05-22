// TaskManagement.Domain/Events/DomainEvent.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Events;

/// <summary>
/// DomainEvent es una clase base abstracta para todos los eventos de dominio en el sistema.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte del core de la aplicación en la capa de dominio</item>
/// <item>Implementa patrón <a href="https://martinfowler.com/bliki/DomainDrivenDesign.html">Domain-Driven Design</a> para arquitectura dirigida por eventos</item>
/// <item>Habilita acoplamiento suelto entre agregados de dominio y servicios de aplicación</item>
/// <item>Captura ocurrencias de negocio importantes que sucedieron en el dominio</item>
/// <item>Permite que múltiples servicios reaccionen a cambios de dominio sin acoplamiento directo</item>
/// </list>
///
/// Eventos de Dominio en este sistema:
/// <list type="bullet">
/// <item>TaskCreatedEvent: Se dispara cuando se crea una nueva tarea</item>
/// <item>TaskCompletedEvent: Se dispara cuando una tarea se marca como completada</item>
/// <item>TaskAssignedEvent: Se dispara cuando una tarea se asigna a un usuario</item>
/// <item>TaskPriorityChangedEvent: Se dispara cuando se cambia prioridad de tarea</item>
/// </list>
///
/// Beneficios:
/// <list type="bullet">
/// <item>Desacopla lógica de dominio de preocupaciones de infraestructura</item>
/// <item>Habilita efectos secundarios (notificaciones, correos) sin modificar entidades de dominio</item>
/// <item>Proporciona pista de auditoría de actividades de dominio</item>
/// <item>Soporta comunicación entre agregados</item>
/// </list>
/// </summary>
public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public class TaskCreatedEvent : DomainEvent
{
    public Guid TaskId
    {
        get;
    }
    public string Title
    {
        get;
    }
    public Guid CreatedBy
    {
        get;
    }
    public TaskCreatedEvent(Guid taskId, string title, Guid createdBy)
    {
        TaskId = taskId;
        Title = title;
        CreatedBy = createdBy;
    }
}

public class TaskCompletedEvent : DomainEvent
{
    public Guid TaskId
    {
        get;
    }
    public DateTime CompletedAt
    {
        get;
    }
    public TaskCompletedEvent(Guid taskId, DateTime completedAt)
    {
        TaskId = taskId;
        CompletedAt = completedAt;
    }
}

public class TaskAssignedEvent : DomainEvent
{
    public Guid TaskId
    {
        get;
    }
    public Guid AssignedTo
    {
        get;
    }
    public Guid? PreviousAssignee
    {
        get;
    }
    public TaskAssignedEvent(Guid taskId, Guid assignedTo, Guid? previousAssignee)
    {
        TaskId = taskId;
        AssignedTo = assignedTo;
        PreviousAssignee = previousAssignee;
    }
}

public class TaskPriorityChangedEvent : DomainEvent
{
    public Guid TaskId
    {
        get;
    }
    public TaskPriority OldPriority
    {
        get;
    }
    public TaskPriority NewPriority
    {
        get;
    }
    public TaskPriorityChangedEvent(Guid taskId, TaskPriority oldPriority, TaskPriority newPriority)
    {
        TaskId = taskId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}
