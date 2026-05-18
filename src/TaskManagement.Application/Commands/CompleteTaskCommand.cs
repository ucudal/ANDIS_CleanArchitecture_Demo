using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Commands.CompleteTask;

public sealed record CompleteTaskCommand(Guid TaskId) : IRequest<Result>;

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
