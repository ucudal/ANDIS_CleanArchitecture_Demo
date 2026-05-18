// TaskManagement.Domain/Interfaces/ITaskRepository.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Update(TaskItem task);
    void Delete(TaskItem task);
}
