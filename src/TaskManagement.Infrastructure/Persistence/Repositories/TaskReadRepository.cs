// TaskManagement.Infrastructure/Persistence/Repositories/TaskReadRepository.cs
namespace TaskManagement.Infrastructure.Persistence.Repositories;

using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Application.Queries.SearchTasks;
using TaskManagement.Application.Shared;

public sealed class TaskReadRepository : ITaskReadRepository
{
    private readonly IDbConnection _connection;
    public TaskReadRepository(IConfiguration configuration)
    {
        _connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
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
                creator.Name as CreatedByName,
                assignee.Name as AssignedToName
            FROM Tasks t
            LEFT JOIN Users creator ON t.CreatedBy = creator.Id
            LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id
            WHERE t.Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<TaskDto>(
            sql,
            new { Id = id });
    }
    public async Task<PagedResult<TaskDto>> SearchAsync(
        TaskSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        const string selectSql = @"
            SELECT
                t.Id, t.Title, t.Description, t.Status, t.Priority,
                t.DueDate, t.CreatedAt, creator.Name as CreatedByName,
                assignee.Name as AssignedToName
            FROM Tasks t
            LEFT JOIN Users creator ON t.CreatedBy = creator.Id
            LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id";
        const string countSql = "SELECT COUNT(*) FROM Tasks t";
        var whereClause = BuildWhereClause(request);
        var orderClause = $"ORDER BY {request.SortBy} {request.SortDirection}";
        var pagedSql = $"{selectSql} {whereClause} {orderClause} LIMIT @PageSize OFFSET @Offset";
        var parameters = new DynamicParameters(request);
        parameters.Add("Offset", (request.Page - 1) * request.PageSize);
        var multi = await _connection.QueryMultipleAsync(
            $"{pagedSql}; {countSql} {whereClause}",
            parameters);
        var items = (await multi.ReadAsync<TaskDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();
        return PagedResult<TaskDto>.Create(items, totalCount, request.Page, request.PageSize);
    }

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