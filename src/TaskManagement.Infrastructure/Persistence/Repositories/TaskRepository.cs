// TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

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
