// TaskManagement.Domain/Events/DomainEvent.cs
namespace TaskManagement.Domain.Events;

using TaskManagement.Domain.Entities;

public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public class TaskCreatedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public string Title { get; }
    public Guid CreatedBy { get; }
    public TaskCreatedEvent(Guid taskId, string title, Guid createdBy)
    {
        TaskId = taskId;
        Title = title;
        CreatedBy = createdBy;
    }
}
public class TaskCompletedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public DateTime CompletedAt { get; }
    public TaskCompletedEvent(Guid taskId, DateTime completedAt)
    {
        TaskId = taskId;
        CompletedAt = completedAt;
    }
}
public class TaskAssignedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid AssignedTo { get; }
    public Guid? PreviousAssignee { get; }
    public TaskAssignedEvent(Guid taskId, Guid assignedTo, Guid? previousAssignee)
    {
        TaskId = taskId;
        AssignedTo = assignedTo;
        PreviousAssignee = previousAssignee;
    }
}
public class TaskPriorityChangedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public TaskPriority OldPriority { get; }
    public TaskPriority NewPriority { get; }
    public TaskPriorityChangedEvent(Guid taskId, TaskPriority oldPriority, TaskPriority newPriority)
    {
        TaskId = taskId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}