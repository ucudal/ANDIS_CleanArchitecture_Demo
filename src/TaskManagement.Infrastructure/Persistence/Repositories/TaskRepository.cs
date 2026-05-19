// TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// TaskRepository is the write repository for TaskItem entities.
///
/// Role in Clean Architecture:
/// - Part of the Infrastructure Layer
/// - Implements ITaskRepository interface (defined in Domain Layer)
/// - Data access implementation: Translates domain operations to database queries
/// - Dependency inversion: Domain depends on interface, not this implementation
///
/// Repository Pattern Benefits:
/// - Abstracts Entity Framework details from domain and application layers
/// - Provides domain-oriented API (not query-focused)
/// - Enables unit testing through mock implementations
/// - Allows swapping database technology without changing application code
///
/// Write Operations vs Read Operations:
/// - This repository: Write operations (Add, Update, Delete)
/// - TaskReadRepository: Read operations (optimized queries)
/// - Separation follows CQRS: Different strategies for reads and writes
///
/// Implementation Details:
/// - Uses Entity Framework Core for database access
/// - AsNoTracking for read-only queries (GetByIdAsync, GetByAssigneeAsync, GetOverdueAsync)
/// - Tracking enabled for write operations (AddAsync, Update, Delete)
/// - Returns immutable collections (IReadOnlyList) to prevent client modifications
///
/// Methods:
/// - GetByIdAsync: Find task by ID (read)
/// - GetByAssigneeAsync: Find tasks assigned to user (read)
/// - GetOverdueAsync: Find incomplete overdue tasks (read)
/// - AddAsync: Persist new task entity (write)
/// - Update: Mark existing task for update (write)
/// - Delete: Mark existing task for deletion (write)
/// </summary>
public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _dbContext;
    public TaskRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking() // Read-only query
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken).ConfigureAwait(false);
    }
    public async Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.AssignedTo == userId)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<IReadOnlyList<TaskItem>> GetOverdueAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate < DateTime.UtcNow
                && t.Status != TaskManagement.Domain.Entities.TaskStatus.Completed)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Tasks.AddAsync(task, cancellationToken).ConfigureAwait(false);
    }
    public void Update(TaskItem task)
    {
        _dbContext.Tasks.Update(task);
    }
    public void Delete(TaskItem task)
    {
        _dbContext.Tasks.Remove(task);
    }
}
