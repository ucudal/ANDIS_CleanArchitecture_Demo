using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Commands.CompleteTask;

/// <summary>
/// CompleteTaskCommand encapsulates the request to mark a task as complete.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - CQRS Command: Represents a request to perform a state-changing operation
/// - Carries input parameters from API/UI layer to application logic
/// - Input DTO for completing a task
/// - Implements MediatR IRequest for dependency injection and middleware processing
///
/// Design Pattern:
/// - Records in C# are immutable by default, preventing accidental modifications
/// - Sealed prevents inheritance, ensuring tight coupling to specific type
/// - Generic IRequest allows strongly-typed results with Result pattern
/// </summary>
public sealed record CompleteTaskCommand(Guid TaskId) : IRequest<Result>;

/// <summary>
/// CompleteTaskCommandHandler is the application service for completing tasks.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Application Service: Orchestrates domain and infrastructure layers
/// - MediatR Handler: Processes commands through a pipeline
/// - Implements business use case logic (not domain logic)
///
/// Responsibilities:
/// - Retrieves task from repository
/// - Delegates state transition to domain entity (Complete method)
/// - Persists changes through unit of work
/// - Dispatches domain events for post-completion actions
///
/// Domain Layer Interaction:
/// - Uses TaskItem.Complete method to enforce business rules
/// - Returns domain-defined errors if business rules are violated
/// - Depends on repository abstraction (no direct database access)
///
/// Error Handling:
/// - Returns Result.Failure if task not found
/// - Returns domain errors if task cannot be completed (already completed, etc.)
/// - Prevents invalid state transitions at domain level
/// </summary>
public sealed class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public CompleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
            return Result.Failure(TaskErrors.NotFound(request.TaskId));

        var result = task.Complete();
        if (result.IsFailure)
            return result;

        // Task was loaded with AsNoTracking; attach it so state changes are persisted.
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();

        return Result.Success();
    }
}
