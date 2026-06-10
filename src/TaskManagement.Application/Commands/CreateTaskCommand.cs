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
/// <li>Parte de la capa de aplicación</li>
/// <li>Un comando en <a
/// href="https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_09_.Patrones_de_arquitectura/2_09_CQRS.md">CQRS</a>
/// representa una solicitud para realizar una operación que cambia estado</li>
/// <li>Transporta parámetros de entrada desde la capa de API a la lógica de
/// aplicación</li>
/// <li>Es un DTO de entrada para crear una tarea</li>
/// <li>Implementa <a href="https://mediatr.io">MediatR</a> IRequest para
/// inyección de dependencias y procesamiento de middleware</li>
/// </ul>
///
/// Patrón CQRS -CommandQueryResponsibilitySeparation-:
/// <ul>
/// <li>Comando: Muta el estado del sistema, como <c>CreateTaskCommand</c></li>
/// <li>Consulta: Lee datos sin efectos secundarios -separada de comandos- como
/// <c>TaskManagement.Application.Queries.GetTaskByIdQuery</c></li>
/// <li>Separación: Permite optimización independiente de operaciones de lectura
/// y escritura</li>
/// <li>Este comando específíco representa la intención del usuario de crear una
/// tarea</li>
/// </ul>
/// </remarks>
public sealed class CreateTaskCommand : IRequest<Result<Guid>>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid CreatedBy { get; set; }

    public CreateTaskCommand(
        string title,
        string description,
        TaskPriority priority,
        DateTime? dueDate,
        Guid createdBy)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// CreateTaskCommandHandler es el servicio de aplicación para manejar la creación de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Servicio de Aplicación: Orquesta las capas del dominio e infraestructura</li>
/// <li>Manejador de <a href="https://mediatr.io">MediatR</a>: Procesa comandos a través de un pipeline</li>
/// <li>Implementa lógica de caso de uso -no lógica del dominio-</li>
/// </ul>
///
/// Responsabilidades:
/// <ul>
/// <li>Valida entrada a través de creación de entidad del dominio</li>
/// <li>Orquesta llamadas de repositorio y unidad de trabajo</li>
/// <li>Envía eventos del dominio después de persistencia</li>
/// <li>Devuelve resultados de éxito o fracaso a la capa de API</li>
/// </ul>
///
/// Interacción de Capa del dominio:
/// <ul>
/// <li>Utiliza TaskItem.Create -fábrica del dominio- para validación de regla de negocio</li>
/// <li>Depende de abstracciones ITaskRepository e IUnitOfWork</li>
/// <li>Aprovecha eventos del dominio para desacoplamiento de la infraestructura y facilidad del testing.</li>
/// </ul>
///
/// Separación de responsabilidades:
/// <ul>
/// <li>NO contiene lógica de negocio -delegada a dominio-</li>
/// <li>NO interactúa directamente con base de datos -utiliza repositorios-</li>
/// <li>NO Maneja cuestiones HTTP -delegadas a controlador-</li>
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
        // Enviar eventos del dominio después de persistencia exitosa
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();
        return Result.Success(task.Id);
    }
}
