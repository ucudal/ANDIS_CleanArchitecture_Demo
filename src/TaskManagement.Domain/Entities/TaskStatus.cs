namespace TaskManagement.Domain.Entities;

/// <summary>
/// TaskStatus is a Domain Value Object (Enum) representing possible states of a task.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Represents core business concepts without external dependencies
/// - Ensures type safety for task states throughout the system
/// - Used by TaskItem entity to enforce valid state transitions
///
/// Valid States:
/// - Todo (0): Initial state for new tasks
/// - InProgress (1): Task is currently being worked on
/// - Completed (2): Task has been finished
/// </summary>
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Completed = 2
}
