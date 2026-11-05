using TaskManagement.Application.Common;
using TaskManagement.Application.Ports;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CreateTaskInteractor</c> implementa el caso de uso de crear una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Orquesta el caso de uso: las reglas de negocio están en la entidad TaskItem</li>
/// <li>Solo conoce puertos: no sabe qué base de datos ni qué interfaz de usuario se usa</li>
/// <li>Entrega el resultado por ICreateTaskOutputBoundary, nunca entrega entidades</li>
/// </ul>
///
/// Flujo:
/// <ol>
/// <li>Crea la entidad, que valida sus invariantes</li>
/// <li>Si alguna regla no se cumple, presenta los errores y termina</li>
/// <li>Persiste la tarea y confirma la unidad de trabajo</li>
/// <li>Publica los eventos del dominio, después de persistir</li>
/// <li>Presenta el identificador de la tarea creada</li>
/// </ol>
/// </remarks>
public sealed class CreateTaskInteractor : ICreateTaskInputBoundary
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ICreateTaskOutputBoundary _output;

    public CreateTaskInteractor(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ICreateTaskOutputBoundary output)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _output = output;
    }

    public async Task ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var createResult = TaskItem.Create(
            input.Title,
            input.Description,
            ToDomainPriority(input.Priority),
            input.DueDate,
            input.CreatedBy);
        if (createResult.IsFailure)
        {
            _output.PresentValidationErrors(createResult.Errors);
            return;
        }

        var task = createResult.Value!;
        await _taskRepository.AddAsync(task, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        // Enviar eventos del dominio después de persistencia exitosa
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();

        _output.PresentSuccess(new CreateTaskOutput(task.Id));
    }

    private static TaskPriority ToDomainPriority(TaskPriorityLevel priority) => priority switch
    {
        TaskPriorityLevel.Low => TaskPriority.Low,
        TaskPriorityLevel.Medium => TaskPriority.Medium,
        TaskPriorityLevel.High => TaskPriority.High,
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Unknown priority.")
    };
}
