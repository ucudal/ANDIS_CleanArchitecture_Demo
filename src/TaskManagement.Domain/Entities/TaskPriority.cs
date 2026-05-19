namespace TaskManagement.Domain.Entities;

/// <summary>
/// TaskPriority is a Domain Value Object (Enum) representing task priority levels.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Represents a core business concept without external dependencies
/// - Ensures type safety for priority levels throughout the system
/// - Used by TaskItem entity to categorize task urgency
///
/// Priority Levels (in ascending order):
/// - Low (0): Task has low urgency
/// - Medium (1): Task has medium urgency
/// - High (2): Task has high urgency and should be prioritized
/// </summary>
public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}
