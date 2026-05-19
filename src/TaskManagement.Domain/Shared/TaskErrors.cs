namespace TaskManagement.Domain.Common;

/// <summary>
/// TaskErrors is a central repository for all task domain error messages.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer - Shared/Common)
/// - Centralizes error messages to avoid duplication and ensure consistency
/// - Represents all possible business rule violations in task management
/// - Used throughout domain and application layers for error reporting
///
/// Benefits:
/// - Single source of truth for error messages
/// - Easy to maintain and update error messages
/// - Supports internationalization (i18n) if needed
/// - Prevents string duplication and typos
/// - Clear documentation of all possible task-related errors
///
/// Error Categories:
/// - Status transition errors (AlreadyCompleted, CannotModifyCompletedTask, etc.)
/// - Assignment errors (CannotAssignCompletedTask)
/// - Resource errors (NotFound)
/// </summary>
public static class TaskErrors
{
    public const string AlreadyCompleted = "Task is already completed.";
    public const string CannotAssignCompletedTask = "Cannot assign a completed task.";
    public const string CannotModifyCompletedTask = "Cannot modify a completed task.";

    public static string NotFound(Guid id) => $"Task with ID '{id}' was not found.";
}
