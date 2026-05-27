// TaskManagement.Infrastructure/Persistence/Repositories/TaskReadRepository.cs
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries;
using TaskManagement.Application.Queries.SearchTasks;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// <c>TaskReadRepository</c> es el repositorio de lectura para consultas de tarea optimizadas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz <see cref="ITaskReadRepository"/> (definida en Capa de Aplicación)</li>
/// <li>Acceso a datos optimizado: Proporciona consultas eficientes de solo lectura</li>
/// <li>Inversión de dependencia: La aplicación depende de interfaz, no de esta implementación</li>
/// </ul>
///
/// Patrón CQRS - Lado de Lectura:
/// <ul>
/// <li>Separado del repositorio de escritura (<see cref="TaskRepository"/>)</li>
/// <li>Optimizado para rendimiento y escalabilidad de consultas</li>
/// <li>Puede usar diferentes tecnologías (<see cref="Dapper"/>, SQL sin procesar, modelos de lectura)</li>
/// <li>Devuelve DTOs en lugar de entidades de dominio</li>
/// </ul>
///
/// Beneficios del Repositorio de Lectura:
/// <ul>
/// <li>Optimización independiente del modelo de escritura</li>
/// <li>Puede usar SQL sin procesar o herramientas de consulta especializadas</li>
/// <li>Puede consultar vistas desnormalizadas o modelos de lectura materializados</li>
/// <li>Devuelve solo campos requeridos (proyección)</li>
/// <li>Soporta estrategias de caché optimizadas para lecturas</li>
/// </ul>
///
/// Beneficios de Separación:
/// <ul>
/// <li>El lado de escritura optimiza para consistencia y reglas de negocio</li>
/// <li>El lado de lectura optimiza para rendimiento de consulta y forma de datos</li>
/// <li>Puede escalar independientemente basado en patrones de lectura/escritura</li>
/// <li>Separa la optimización de lecturas del lado de escritura (patrón CQRS)</li>
/// </ul>
///
/// Implementación:
/// <ul>
/// <li>Utiliza interfaz <see cref="ITaskReadRepository"/> de Capa de Aplicación</li>
/// <li>Típicamente utiliza <see cref="Dapper"/> o Entity Framework con <c>AsNoTracking</c></li>
/// <li>Devuelve <see cref="IReadOnlyList{T}"/> para operaciones de lectura</li>
/// <li>Maneja paginación para conjuntos de resultados grandes</li>
/// </ul>
/// </remarks>
public sealed class TaskReadRepository : ITaskReadRepository
{
    private readonly string _connectionString;

    public TaskReadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public async Task<TaskDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.DueDate,
                t.CreatedAt,
                t.CreatedBy as CreatedBy,
                t.AssignedTo as AssignedTo
            FROM Tasks t
            WHERE t.Id = @Id";

        using var connection = CreateConnection();
        var command = new CommandDefinition(
            sql,
            new
            {
                Id = id
            },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<TaskDto>(command).ConfigureAwait(false);
    }

    public async Task<PagedResult<TaskDto>> SearchAsync(
        TaskSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        const string selectSql = @"
            SELECT
                t.Id, t.Title, t.Description, t.Status, t.Priority,
                t.DueDate, t.CreatedAt, t.CreatedBy as CreatedBy,
                t.AssignedTo as AssignedTo
            FROM Tasks t";
        const string countSql = "SELECT COUNT(*) FROM Tasks t";
        var whereClause = BuildWhereClause(request);
        var orderClause = $"ORDER BY {request.SortBy} {request.SortDirection}";
        var pagedSql = $"{selectSql} {whereClause} {orderClause} LIMIT @PageSize OFFSET @Offset";
        var parameters = new DynamicParameters(request);
        parameters.Add("Offset", (request.Page - 1) * request.PageSize);

        using var connection = CreateConnection();
        var command = new CommandDefinition(
            $"{pagedSql}; {countSql} {whereClause}",
            parameters,
            cancellationToken: cancellationToken);

        var multi = await connection.QueryMultipleAsync(command).ConfigureAwait(false);
        var items = (await multi.ReadAsync<TaskDto>().ConfigureAwait(false)).ToList();
        var totalCount = await multi.ReadSingleAsync<int>().ConfigureAwait(false);
        return new PagedResult<TaskDto>(items, totalCount, request.Page, request.PageSize);
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    private static string BuildWhereClause(TaskSearchRequest request)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            filters.Add("t.Title LIKE '%' || @Title || '%'");
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filters.Add("t.Status = @Status");
        }

        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            filters.Add("t.Priority = @Priority");
        }

        if (request.AssignedTo.HasValue)
        {
            filters.Add("t.AssignedTo = @AssignedTo");
        }

        return filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}";
    }
}
