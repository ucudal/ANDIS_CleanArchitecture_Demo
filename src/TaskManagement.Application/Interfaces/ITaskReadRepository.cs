using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// ITaskReadRepository is the abstraction for read-only task queries.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Defines contract for efficient task data retrieval
/// - Implementation abstraction: Interface in Application Core, implementation in Infrastructure
/// - Enables CQRS (Command Query Responsibility Segregation) pattern
///
/// Read Repository Separation Benefits:
/// - Optimized for read performance (can use Dapper, raw SQL, read models)
/// - Independent from write repository (ITaskRepository)
/// - Supports eventual consistency patterns
/// - Can denormalize data for query efficiency
/// - Enables different optimization strategies for reads vs writes
///
/// Contrast with ITaskRepository:
/// - ITaskRepository: Write operations (Add, Update, Delete)
/// - ITaskReadRepository: Read operations optimized for queries
/// - Follows CQRS: Separates command (write) from query (read) responsibilities
///
/// Implementation Strategy:
/// - Can implement using Entity Framework AsNoTracking for efficiency
/// - Can implement using Dapper for fine-grained SQL control
/// - Can query read-optimized denormalized tables or views
/// - Returns DTOs optimized for specific query use cases
/// </summary>

public interface ITaskReadRepository
{
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
