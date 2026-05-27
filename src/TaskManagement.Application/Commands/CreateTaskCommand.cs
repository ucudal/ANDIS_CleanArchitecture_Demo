// TaskManagement.Application/Commands/CreateTask/CreateTaskCommand.cs
using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CreateTaskCommand</c> encapsula la solicitud de creación de una nueva tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>Comando CQRS: Representa una solicitud para realizar una operación que cambia estado</li>
/// <li>Transporta parámetros de entrada desde la capa de API/UI a la lógica de aplicación</li>
/// <li>Objeto de Transferencia de Datos (DTO) de entrada para crear una tarea</li>
/// <li>Implementa <see cref="MediatR"/> <see cref="IRequest"/> para inyección de dependencias y procesamiento de middleware</li>
/// </ul>
///
/// Patrón CQRS —Command Query Responsibility Segregation—:
/// <ul>
/// <li>Comando: Muta el estado del sistema (CreateTaskCommand)</li>
/// <li>Consulta: Lee datos sin efectos secundarios (separada de comandos)</li>
/// <li>Separación: Permite optimización independiente de operaciones de lectura y escritura</li>
/// <li>Este comando específíco representa la intención del usuario de crear una tarea</li>
/// </ul>
/// </remarks>
public sealed record CreateTaskCommand(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid CreatedBy
) : IRequest<Result<Guid>>;

/// <summary>
/// CreateTaskCommandHandler es el servicio de aplicación para manejar la creación de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>Servicio de Aplicación: Orquesta las capas de dominio e infraestructura</li>
/// <li>Manejador de <see cref="MediatR"/>: Procesa comandos a través de un pipeline</li>
/// <li>Implementa lógica de caso de uso (no lógica de dominio)</li>
/// </ul>
///
/// Responsabilidades:
/// <ul>
/// <li>Valida entrada a través de creación de entidad de dominio</li>
/// <li>Orquesta llamadas de repositorio y unidad de trabajo</li>
/// <li>Envía eventos de dominio después de persistencia</li>
/// <li>Devuelve resultados de éxito/fracaso a la capa de API</li>
/// </ul>
///
/// Interacción de Capa de Dominio:
/// <ul>
/// <li>Utiliza TaskItem.Create (fábrica de dominio) para validación de regla de negocio</li>
/// <li>Depende de abstracciones ITaskRepository e IUnitOfWork</li>
/// <li>Aprovecha eventos de dominio para desacoplamiento de la infraestructura y facilidad del testing.</li>
/// </ul>
///
/// Separación de responsabilidades:
/// <ul>
/// <li>NO contiene lógica de negocio (delegada a dominio)</li>
/// <li>NO interactúa directamente con base de datos (utiliza repositorios)</li>
/// <li>NO Maneja cuestiones HTTP (delegadas a controlador)</li>
/// <li>Coordina entre capas para cumplir el caso de uso</li>
/// </ul>
/// </remarks>
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
        // Enviar eventos de dominio después de persistencia exitosa
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();
        return Result.Success(task.Id);
    }
}
