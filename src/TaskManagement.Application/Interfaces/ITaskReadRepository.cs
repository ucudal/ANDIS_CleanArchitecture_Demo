namespace TaskManagement.Application.Interfaces;

using TaskManagement.Application.Queries.GetTaskById;

public interface ITaskReadRepository
{
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
