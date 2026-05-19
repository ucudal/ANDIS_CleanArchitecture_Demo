// TaskManagement.Application/Commands/CreateTask/CreateTaskCommand.cs
using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Commands.CreateTask;

/// <summary>
/// CreateTaskCommand encapsulates the request to create a new task.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - CQRS Command: Represents a request to perform a state-changing operation
/// - Carries input parameters from API/UI layer to application logic
/// - Input Data Transfer Object (DTO) for creating a task
/// - Implements MediatR IRequest for dependency injection and middleware processing
///
/// CQRS Pattern (Command Query Responsibility Segregation):
/// - Command: Mutates system state (CreateTaskCommand)
/// - Query: Reads data without side effects (separate from commands)
/// - Separation: Enables independent optimization of read and write operations
/// - This specific command represents the user's intent to create a task
/// </summary>
public sealed record CreateTaskCommand(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid CreatedBy
) : IRequest<Result<Guid>>;

/// <summary>
/// CreateTaskCommandHandler is the application service for handling task creation.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Application Service: Orchestrates domain and infrastructure layers
/// - MediatR Handler: Processes commands through a pipeline
/// - Implements business use case logic (not domain logic)
///
/// Responsibilities:
/// - Validates input through domain entity creation
/// - Orchestrates repository and unit of work calls
/// - Dispatches domain events after persistence
/// - Returns success/failure results to the API layer
///
/// Domain Layer Interaction:
/// - Uses TaskItem.Create (domain factory) for business rule validation
/// - Depends on ITaskRepository and IUnitOfWork abstractions
/// - Leverages domain events for eventual consistency
///
/// Separation of Concerns:
/// - Does NOT contain business logic (delegated to domain)
/// - Does NOT interact directly with database (uses repositories)
/// - Does NOT handle HTTP concerns (delegated to controller)
/// - Coordinates between layers to fulfill use case
/// </summary>
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<Guid>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var createResult = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatedBy);
        if (createResult.IsFailure)
            return Result.Failure<Guid>(createResult.Errors);
        var task = createResult.Value!;
        await _taskRepository.AddAsync(task, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        // Dispatch domain events after successful persistence
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();
        return Result.Success(task.Id);
    }
}
