namespace TaskManagement.Application.Queries.SearchTasks;

public sealed record TaskSearchRequest(
    string? Title = null,
    string? Status = null,
    string? Priority = null,
    Guid? AssignedTo = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "CreatedAt",
    string SortDirection = "DESC"
);
