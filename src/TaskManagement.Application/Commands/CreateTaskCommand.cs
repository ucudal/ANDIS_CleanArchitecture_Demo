// TaskManagement.Application/Commands/CreateTask/CreateTaskCommand.cs
using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands.CreateTask;

/// <summary>
/// <c>CreateTaskCommand</c> encapsula la solicitud de creación de una nueva tarea.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Comando CQRS: Representa una solicitud para realizar una operación que cambia estado
/// - Transporta parámetros de entrada desde la capa de API/UI a la lógica de aplicación
/// - Objeto de Transferencia de Datos (DTO) de entrada para crear una tarea
/// - Implementa <see cref="MediatR"/> <see cref="IRequest"/> para inyección de dependencias y procesamiento de middleware
///
/// Patrón CQRS —Command Query Responsibility Segregation—:
/// - Comando: Muta el estado del sistema (CreateTaskCommand)
/// - Consulta: Lee datos sin efectos secundarios (separada de comandos)
/// - Separación: Permite optimización independiente de operaciones de lectura y escritura
/// - Este comando específíco representa la intención del usuario de crear una tarea
/// </summary>
public sealed record CreateTaskCommand(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid CreatedBy
) : IRequest<Result<Guid>>;

/// <summary>
/// CreateTaskCommandHandler es el servicio de aplicación para manejar la creación de tareas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Servicio de Aplicación: Orquesta las capas de dominio e infraestructura
/// - Manejador de <see cref="MediatR"/>: Procesa comandos a través de un pipeline
/// - Implementa lógica de caso de uso (no lógica de dominio)
///
/// Responsabilidades:
/// - Valida entrada a través de creación de entidad de dominio
/// - Orquesta llamadas de repositorio y unidad de trabajo
/// - Envía eventos de dominio después de persistencia
/// - Devuelve resultados de éxito/fracaso a la capa de API
///
/// Interacción de Capa de Dominio:
/// - Utiliza TaskItem.Create (fábrica de dominio) para validación de regla de negocio
/// - Depende de abstracciones ITaskRepository e IUnitOfWork
/// - Aprovecha eventos de dominio para desacoplamiento de la infraestructura y facilidad del testing.
///
/// Separación de Responsabilidades:
/// - NO contiene lógica de negocio (delegada a dominio)
/// - NO interactúa directamente con base de datos (utiliza repositorios)
/// - NO maneja preocupaciones HTTP (delegadas a controlador)
/// - Coordina entre capas para cumplir el caso de uso
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
        // Enviar eventos de dominio después de persistencia exitosa
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();
        return Result.Success(task.Id);
    }
}
