using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;
public sealed class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly ITaskReadRepository _readRepository;
    public GetTaskByIdQueryHandler(ITaskReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<Result<TaskDto>> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var task = await _readRepository.GetByIdAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
            return Result.Failure<TaskDto>(TaskErrors.NotFound(request.TaskId));
        return Result.Success(task);
    }
}
// DTO optimized for reads. Keep it as a property-based type so Dapper can
// materialize without requiring an exact constructor signature from provider types.
public sealed class TaskDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime? DueDate
    {
        get; init;
    }
    public DateTime CreatedAt
    {
        get; init;
    }
    public string CreatedBy { get; init; } = string.Empty;
    public string? AssignedTo
    {
        get; init;
    }
}
