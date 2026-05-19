// TaskManagement.Domain/Events/DomainEvent.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Events;

/// <summary>
/// DomainEvent is an abstract base class for all domain events in the system.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Implements Domain-Driven Design (DDD) pattern for event-driven architecture
/// - Enables loose coupling between domain aggregates and application services
/// - Captures important business occurrences that happened in the domain
/// - Supports eventual consistency and asynchronous processing
///
/// Domain Events in this system:
/// - TaskCreatedEvent: Fired when a new task is created
/// - TaskCompletedEvent: Fired when a task is marked as complete
/// - TaskAssignedEvent: Fired when a task is assigned to a user
/// - TaskPriorityChangedEvent: Fired when task priority is changed
///
/// Benefits:
/// - Decouples domain logic from infrastructure concerns
/// - Enables side effects (notifications, emails) without modifying domain entities
/// - Provides audit trail of domain activities
/// - Supports cross-aggregate communication
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
