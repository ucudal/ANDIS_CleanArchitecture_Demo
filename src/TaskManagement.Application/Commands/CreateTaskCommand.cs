// TaskManagement.Application/Commands/CreateTask/CreateTaskCommand.cs
using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid CreatedBy
) : IRequest<Result<Guid>>;

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
