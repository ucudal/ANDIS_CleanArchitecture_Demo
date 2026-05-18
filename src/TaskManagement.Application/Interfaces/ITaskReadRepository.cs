using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.Application.Interfaces;

public interface ITaskReadRepository
{
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
