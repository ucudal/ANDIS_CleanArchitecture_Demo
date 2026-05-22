using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Commands.CompleteTask;

/// <summary>
/// <c>CompleteTaskCommand</c> encapsula la solicitud de marcar una tarea como completada.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Comando CQRS: Representa una solicitud para realizar una operación que cambia estado
/// - Transporta parámetros de entrada desde la capa de API/UI a la lógica de aplicación
/// - DTO de entrada para completar una tarea
/// - Implementa <see cref="MediatR"/> <see cref="IRequest"/> para inyección de dependencias y procesamiento de middleware
///
/// Patrón de Diseño:
/// - Los registros en C# son inmutables por defecto, previniendo modificaciones accidentales
/// - Sellado previene herencia, asegurando acoplamiento fuerte a tipo específicos
/// - <see cref="IRequest"/> genérico permite resultados fuertemente tipificados con patrón <see cref="Result"/>
/// </summary>
public sealed record CompleteTaskCommand(Guid TaskId) : IRequest<Result>;

/// <summary>
/// CompleteTaskCommandHandler es el servicio de aplicación para completar tareas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Servicio de Aplicación: Orquesta las capas de dominio e infraestructura
/// - Manejador de <see cref="MediatR"/>: Procesa comandos a través de un pipeline
/// - Implementa lógica de caso de uso (no lógica de dominio)
///
/// Responsabilidades:
/// - Recupera tarea del repositorio
/// - Delega transición de estado a entidad de dominio (método Complete)
/// - Persiste cambios a través de unidad de trabajo
/// - Envía eventos de dominio para acciones posteriores a finalización
///
/// Interacción de Capa de Dominio:
/// - Utiliza método TaskItem.Complete para aplicar reglas de negocio
/// - Devuelve errores definidos por dominio si se violan reglas de negocio
/// - Depende de abstracción de repositorio (sin acceso directo a base de datos)
///
/// Manejo de Errores:
/// - Devuelve Result.Failure si tarea no se encuentra
/// - Devuelve errores de dominio si tarea no puede completarse (ya completada, etc.)
/// - Previene transiciones de estado inválidas a nivel de dominio
/// </summary>
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
