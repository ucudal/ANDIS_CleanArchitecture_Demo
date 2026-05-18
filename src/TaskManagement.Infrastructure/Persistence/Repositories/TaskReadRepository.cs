// TaskManagement.Infrastructure/Persistence/Repositories/TaskReadRepository.cs
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Common;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Application.Queries.SearchTasks;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

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
