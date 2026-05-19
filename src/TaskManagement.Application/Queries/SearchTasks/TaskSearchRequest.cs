namespace TaskManagement.Application.Queries.SearchTasks;

/// <summary>
/// TaskSearchRequest is the input DTO for advanced task search operations.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Input DTO: Transfers search filter data to query handlers
/// - Specification pattern: Encapsulates filtering and sorting logic
/// - Supports complex querying with multiple filter options
///
/// Filtering Capabilities:
/// - Title: Filter tasks by partial title match
/// - Status: Filter tasks by current status
/// - Priority: Filter tasks by priority level
/// - AssignedTo: Filter tasks assigned to specific user
///
/// Pagination Support:
/// - Page: Current page number (1-based)
/// - PageSize: Number of items per page (default 20)
/// - Enables efficient handling of large result sets
/// - Prevents loading entire database at once
///
/// Sorting Support:
/// - SortBy: Column name to sort by (default "CreatedAt")
/// - SortDirection: "ASC" or "DESC" (default "DESC")
/// - Allows flexible result ordering
/// - Client controls sort order
///
/// Filtering Semantics:
/// - Null values: Optional filters, only applied if provided
/// - Default values: Page=1, PageSize=20, SortBy="CreatedAt", SortDirection="DESC"
/// - Empty results: Valid response if no tasks match filters
///
/// Design Pattern - Specification Pattern:
/// - Encapsulates query criteria in a single type
/// - Type-safe: No string parsing or manipulation
/// - Reusable: Can be used by multiple query handlers
/// - Testable: Can test search logic in isolation
///
/// Usage Flow:
/// 1. Client sends HTTP GET with search parameters
/// 2. Controller deserializes to TaskSearchRequest
/// 3. Query handler uses criteria to build database query
/// 4. Repository executes filtered query
/// 5. Returns PagedResult&lt;TaskDto&gt; with filtered results
/// </summary>
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
