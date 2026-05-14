// TaskManagement.Application/Commands/CreateTask/CreateTaskCommand.cs
namespace TaskManagement.Application.Commands.CreateTask;

using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Domain.Shared;

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
        var createResult = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatedBy);
        if (createResult.IsFailure)
            return Result<Guid>.Failure(createResult.Errors);
        var task = createResult.Value!;
        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Dispatch domain events after successful persistence
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);
        task.ClearDomainEvents();
        return Result<Guid>.Success(task.Id);
    }
}
