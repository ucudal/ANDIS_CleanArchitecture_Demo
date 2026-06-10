using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>GetTaskByIdQuery</c> encapsula una solicitud para recuperar una sola tarea por ID.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Consulta CQRS: Representa una solicitud de lectura de datos sin efectos secundarios</li>
/// <li>Transporta parámetros de entrada desde la capa de API/UI a la lógica de consulta</li>
/// <li>DTO de entrada para recuperación de detalles de tarea</li>
/// <li>Implementa <a href="https://mediatr.io">MediatR</a> IRequest para inyección de dependencias y procesamiento de middleware</li>
/// </ul>
///
/// Patrón CQRS -Lado de Consulta-:
/// <ul>
/// <li>Las consultas no modifican el estado del sistema</li>
/// <li>Las consultas devuelven datos a través de DTOs -TaskDto-</li>
/// <li>Las consultas pueden utilizar modelos de lectura optimizados -ej. Dapper en lugar de Entity Framework-</li>
/// <li>Separación de comandos habilita escalado y optimización independientes</li>
/// </ul>
/// </remarks>
public sealed class GetTaskByIdQuery : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }

    public GetTaskByIdQuery(Guid taskId)
    {
        TaskId = taskId;
    }
}

/// <summary>
/// GetTaskByIdQueryHandler es el servicio de aplicación para recuperar detalles de tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Servicio de Aplicación: Recupera datos de repositorios de lectura</li>
/// <li>Manejador de <a href="https://mediatr.io">MediatR</a>: Procesa consultas a través de un pipeline</li>
/// <li>Implementa lógica de caso de uso de consulta</li>
/// </ul>
///
/// Responsabilidades:
/// <ul>
/// <li>Delega a ITaskReadRepository para lectura optimizada</li>
/// <li>Mapea resultados de base de datos a DTO para consumo de API</li>
/// <li>Devuelve resultado de fracaso si tarea no se encuentra</li>
/// <li>Maneja errores adecuadamente</li>
/// </ul>
///
/// Interacción de Infraestructura:
/// <ul>
/// <li>Utiliza ITaskReadRepository -separada del repositorio de escritura-</li>
/// <li>Desacopla de tecnología de persistencia -podría usar Dapper, SQL, etc.-</li>
/// <li>Implementa patrón CQRS para optimización de lecturas independiente</li>
/// </ul>
///
/// Patrón DTO:
/// <ul>
/// <li>Devuelve TaskDto -modelo de vista- no entidad del dominio</li>
/// <li>Desacopla contratos de API de cambios de modelo del dominio</li>
/// <li>Incluye solo datos necesarios para el caso de uso específfico</li>
/// <li>Optimizado para operaciones de lectura y serialización</li>
/// </ul>
/// </remarks>
public sealed class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly ITaskReadRepository _readRepository;
    public GetTaskByIdQueryHandler(ITaskReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<Result<TaskDto>> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var task = await _readRepository.GetByIdAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
            return Result.Failure<TaskDto>(TaskErrors.NotFound(request.TaskId));
        return Result.Success(task);
    }
}
// DTO optimizado para lecturas. Mantenerlo como tipo basado en propiedades para que Dapper pueda
// materializar sin requerir una firma de constructor exacta de tipos de proveedor.
public sealed class TaskDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime? DueDate {
        get; init;
    }
    public DateTime CreatedAt {
        get; init;
    }
    public string CreatedBy { get; init; } = string.Empty;
    public string? AssignedTo {
        get; init;
    }
}
