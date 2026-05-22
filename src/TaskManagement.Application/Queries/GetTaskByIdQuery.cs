using MediatR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Queries.GetTaskById;

/// <summary>
/// GetTaskByIdQuery encapsula una solicitud para recuperar una sola tarea por ID.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Consulta CQRS: Representa una solicitud de lectura de datos sin efectos secundarios
/// - Transporta parámetros de entrada desde la capa de API/UI a la lógica de consulta
/// - DTO de entrada para recuperación de detalles de tarea
/// - Implementa <see cref="MediatR"/> <see cref="IRequest"/> para inyección de dependencias y procesamiento de middleware
///
/// Patrón CQRS - Lado de Consulta:
/// - Las consultas no modifican el estado del sistema
/// - Las consultas devuelven datos a través de DTOs (<see cref="TaskDto"/>)
/// - Las consultas pueden utilizar modelos de lectura optimizados (ej. Dapper en lugar de Entity Framework)
/// - Separación de comandos habilita escalado y optimización independientes
/// </summary>
public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;

/// <summary>
/// GetTaskByIdQueryHandler es el servicio de aplicación para recuperar detalles de tarea.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Servicio de Aplicación: Recupera datos de repositorios de lectura
/// - Manejador de <see cref="MediatR"/>: Procesa consultas a través de un pipeline
/// - Implementa lógica de caso de uso de consulta
///
/// Responsabilidades:
/// - Delega a <see cref="ITaskReadRepository"/> para lectura optimizada
/// - Mapea resultados de base de datos a DTO para consumo de API
/// - Devuelve resultado de fracaso si tarea no se encuentra
/// - Maneja errores adecuadamente
///
/// Interacción de Infraestructura:
/// - Utiliza ITaskReadRepository (separada del repositorio de escritura)
/// - Desacopla de tecnología de persistencia (podría usar Dapper, SQL, etc.)
/// - Implementa patrón CQRS para optimización de lecturas independiente
///
/// Patrón DTO:
/// - Devuelve TaskDto (modelo de vista) no entidad de dominio
/// - Desacopla contratos de API de cambios de modelo de dominio
/// - Incluye solo datos necesarios para el caso de uso específfico
/// - Optimizado para operaciones de lectura y serialización
/// </summary>
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
    public DateTime? DueDate
    {
        get; init;
    }
    public DateTime CreatedAt
    {
        get; init;
    }
    public string CreatedBy { get; init; } = string.Empty;
    public string? AssignedTo
    {
        get; init;
    }
}
