using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>CompleteTaskCommand</c> encapsula la solicitud de marcar una tarea como completada.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>Comando CQRS: Representa una solicitud para realizar una operación que cambia estado</li>
/// <li>Transporta parámetros de entrada desde la capa de API/UI a la lógica de aplicación</li>
/// <li>DTO de entrada para completar una tarea</li>
/// <li>Implementa <see cref="MediatR"/> <see cref="IRequest"/> para inyección de dependencias y procesamiento de middleware</li>
/// </ul>
///
/// Patrón de Diseño:
/// <ul>
/// <li>Los registros en C# son inmutables por defecto, previniendo modificaciones accidentales</li>
/// <li>Sellado previene herencia, asegurando acoplamiento fuerte a tipo específicos</li>
/// <li><see cref="IRequest"/> genérico permite resultados fuertemente tipificados con patrón <see cref="Result"/></li>
/// </ul>
/// </remarks>
public sealed record CompleteTaskCommand(Guid TaskId) : IRequest<Result>;

/// <summary>
/// CompleteTaskCommandHandler es el servicio de aplicación para completar tareas.
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
/// <li>Recupera tarea del repositorio</li>
/// <li>Delega transición de estado a entidad de dominio (método Complete)</li>
/// <li>Persiste cambios a través de unidad de trabajo</li>
/// <li>Envía eventos de dominio para acciones posteriores a finalización</li>
/// </ul>
///
/// Interacción de Capa de Dominio:
/// <ul>
/// <li>Utiliza método TaskItem.Complete para aplicar reglas de negocio</li>
/// <li>Devuelve errores definidos por dominio si se violan reglas de negocio</li>
/// <li>Depende de abstracción de repositorio (sin acceso directo a base de datos)</li>
/// </ul>
///
/// Manejo de Errores:
/// <ul>
/// <li>Devuelve Result.Failure si tarea no se encuentra</li>
/// <li>Devuelve errores de dominio si tarea no puede completarse (ya completada, etc.)</li>
/// <li>Previene transiciones de estado inválidas a nivel de dominio</li>
/// </ul>
/// </remarks>
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

        // La tarea fue cargada con AsNoTracking; adjuntarla para que los cambios de estado se persistan.
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken).ConfigureAwait(false);
        task.ClearDomainEvents();

        return Result.Success();
    }
}
