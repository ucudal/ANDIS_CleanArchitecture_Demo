// TaskManagement.Domain/Interfaces/ITaskRepository.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// ITaskRepository is the write repository interface for TaskItem aggregate.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Defines the contract for persisting task entities
/// - Implementation abstraction: Interface belongs to Domain, implementation in Infrastructure
/// - Dependency inversion: Domain does not depend on specific persistence technology
///
/// Repository Pattern Benefits:
/// - Abstracts data access logic from domain and application layers
/// - Enables swapping database implementation without changing domain code
/// - Simplifies unit testing by allowing mock implementations
/// - Provides a familiar domain-oriented API (not query-focused)
///
/// Operations:
/// - GetByIdAsync: Retrieve a single task by its ID
/// - GetByAssigneeAsync: Find all tasks assigned to a user
/// - GetOverdueAsync: Find tasks that are past due and not completed
/// - AddAsync: Save a new task to persistence
/// - Update: Modify an existing task
/// - Delete: Remove a task from persistence
///
/// Note: This is the write repository. For read operations, see ITaskReadRepository.
/// This separation follows CQRS (Command Query Responsibility Segregation) pattern.
/// </summary>
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}
