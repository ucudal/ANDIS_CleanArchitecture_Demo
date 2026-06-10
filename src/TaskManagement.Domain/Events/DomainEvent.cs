// TaskManagement.Domain/Events/DomainEvent.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Events;

/// <summary>
/// <c>DomainEvent</c> es una clase base abstracta para todos los <a
/// href="https://github.com/ucudal/ANDIS_Conceptos/blob/main/4_Conceptos/4_Evento_del_dominio.md">eventos
/// del dominio</a> en el sistema.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa del dominio</li>
/// <li>Implementa eventos para arquitectura dirigida por eventos</li>
/// <li>Habilita acoplamiento laxo entre agregados del dominio y servicios de
/// aplicación</li>
/// <li>Captura sucesos del negocio importantes que ocurrieron en el
/// dominio</li>
/// <li>Permite que múltiples servicios reaccionen a cambios del dominio sin
/// acoplamiento directo</li>
/// </ul>
///
/// Eventos del dominio en esta demo:
/// <ul>
/// <li>TaskCreatedEvent: Se dispara cuando se crea una nueva tarea</li>
/// <li>TaskCompletedEvent: Se dispara cuando una tarea se marca como
/// completada</li>
/// <li>TaskAssignedEvent: Se dispara cuando una tarea se asigna a un
/// usuario</li>
/// <li>TaskPriorityChangedEvent: Se dispara cuando se cambia prioridad de
/// tarea</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Desacopla lógica del dominio de preocupaciones de infraestructura</li>
/// <li>Habilita efectos secundarios —notificaciones, correos— sin modificar
/// entidades del dominio</li>
/// <li>Proporciona trazas de auditoría de actividades del dominio</li>
/// <li>Soporta comunicación entre agregados</li>
/// </ul>
/// </remarks>
public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public class TaskCreatedEvent : DomainEvent
{
    public Guid TaskId {
        get;
    }
    public string Title {
        get;
    }
    public Guid CreatedBy {
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
    public Guid TaskId {
        get;
    }
    public DateTime CompletedAt {
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
    public Guid TaskId {
        get;
    }
    public Guid AssignedTo {
        get;
    }
    public Guid? PreviousAssignee {
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
    public Guid TaskId {
        get;
    }
    public TaskPriority OldPriority {
        get;
    }
    public TaskPriority NewPriority {
        get;
    }
    public TaskPriorityChangedEvent(Guid taskId, TaskPriority oldPriority, TaskPriority newPriority)
    {
        TaskId = taskId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}
