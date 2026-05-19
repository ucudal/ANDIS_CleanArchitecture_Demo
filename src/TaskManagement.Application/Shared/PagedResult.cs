namespace TaskManagement.Application.Common;

/// <summary>
/// PagedResult is a DTO for returning paginated query results.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Output DTO: Transfers paginated data from application to API/UI layer
/// - Read Model: Optimized for queries that return multiple items
/// - Supports pagination: Enables efficient handling of large result sets
///
/// Pagination Benefits:
/// - Reduces memory usage: Returns subset of results instead of all
/// - Improves performance: Network bandwidth for smaller payloads
/// - Better UX: Clients can load data incrementally
/// - Scalability: Handles databases with millions of records
///
/// Properties:
/// - Items: Actual data for current page
/// - TotalCount: Total number of items across all pages
/// - Page: Current page number (1-based)
/// - PageSize: Number of items per page
/// - Computed: TotalPages, HasNextPage, HasPreviousPage
///
/// Usage Pattern:
/// - Query handler queries repository for total count and paged items
/// - Returns PagedResult&lt;TaskDto&gt; to controller
/// - Controller serializes to JSON with pagination metadata
/// - Client uses pagination metadata to request next page
///
/// Design:
/// - Immutable: Data set in constructor, cannot change
/// - Generic: PagedResult&lt;T&gt; works with any item type
/// - Sealed: Prevents accidental inheritance
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items
    {
        get;
    }
    public int TotalCount
    {
        get;
    }
    public int Page
    {
        get;
    }
    public int PageSize
    {
        get;
    }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
