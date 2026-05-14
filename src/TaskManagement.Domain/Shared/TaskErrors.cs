namespace TaskManagement.Domain.Shared;

public static class TaskErrors
{
    public const string AlreadyCompleted = "Task is already completed.";
    public const string CannotAssignCompletedTask = "Cannot assign a completed task.";
    public const string CannotModifyCompletedTask = "Cannot modify a completed task.";

    public static string NotFound(Guid id) => $"Task with ID '{id}' was not found.";
}
