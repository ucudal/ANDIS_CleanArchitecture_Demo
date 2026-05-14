namespace TaskManagement.Domain.Entities;

using TaskManagement.Domain.Events;
using TaskManagement.Domain.Shared;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? AssignedTo { get; private set; }
    // Domain events for eventual consistency
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    // EF Core protected constructor
    protected TaskItem() { }
    public static Result<TaskItem> Create(
        string title,
        string description,
        TaskPriority priority,
        DateTime? dueDate,
        Guid createdBy)
    {
        var validation = Validate(title, description, dueDate);
        if (validation.IsFailure)
            return Result<TaskItem>.Failure(validation.Errors);
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
        return Result<TaskItem>.Success(task);
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
        return errors.Any()
            ? Result.Failure(errors)
            : Result.Success();
    }
}