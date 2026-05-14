namespace TaskManagement.Application.Queries.GetTaskById;

using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Shared;


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
        var task = await _readRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound(request.TaskId));
        return Result<TaskDto>.Success(task);
    }
}
// DTO optimized for reads
public sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    string CreatedByName,
    string? AssignedToName
);