using TaskManagement.Application.Ports;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CompleteTaskInteractor</c> implementa el caso de uso de completar una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Orquesta el caso de uso: la regla de negocio está en TaskItem.Complete</li>
/// <li>Distingue los resultados posibles con ICompleteTaskOutputBoundary: el presenter no tiene
/// que interpretar mensajes de error</li>
/// </ul>
/// </remarks>
public sealed class CompleteTaskInteractor : ICompleteTaskInputBoundary
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ICompleteTaskOutputBoundary _output;

    public CompleteTaskInteractor(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ICompleteTaskOutputBoundary output)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _output = output;
    }

    public async Task ExecuteAsync(CompleteTaskInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var task = await _taskRepository.GetByIdAsync(input.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            _output.PresentNotFound(TaskErrors.NotFound(input.TaskId));
            return;
        }

        var result = task.Complete();
        if (result.IsFailure)
        {
            _output.PresentRuleViolation(result.Errors);
            return;
        }

        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();

        _output.PresentSuccess();
    }
}
