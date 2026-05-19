using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Queries.GetTaskById;

/// <summary>
/// GetTaskByIdQuery encapsulates a request to retrieve a single task by ID.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - CQRS Query: Represents a request to read data without side effects
/// - Carries input parameters from API/UI layer to query logic
/// - Input DTO for retrieving task details
/// - Implements MediatR IRequest for dependency injection and middleware processing
///
/// CQRS Pattern - Query Side:
/// - Queries do not modify system state
/// - Queries return data through DTOs (TaskDto)
/// - Queries can use optimized read models (e.g., Dapper instead of Entity Framework)
/// - Separation from commands enables independent scaling and optimization
/// </summary>
public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;

/// <summary>
/// GetTaskByIdQueryHandler is the application service for retrieving task details.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Application Service: Retrieves data from read repositories
/// - MediatR Handler: Processes queries through a pipeline
/// - Implements query use case logic
///
/// Responsibilities:
/// - Delegates to ITaskReadRepository for optimized reading
/// - Maps database results to DTO for API consumption
/// - Returns failure result if task not found
/// - Handles errors gracefully
///
/// Infrastructure Interaction:
/// - Uses ITaskReadRepository (separate from write repository)
/// - Decouples from persistence technology (could use Dapper, SQL, etc.)
/// - Supports eventual consistency patterns
///
/// DTO Pattern:
/// - Returns TaskDto (view model) not domain entity
/// - Decouples API contracts from domain model changes
/// - Includes only data needed for the specific use case
/// - Optimized for read operations and serialization
/// </summary>
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
