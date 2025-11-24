using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetAllSupports;

/// <summary>
/// Represents a query to retrieve a paginated, filtered, and sorted list of support tickets.
/// </summary>
/// <param name="SearchPhrase">An optional search term to filter results by (e.g., in Title or Content).</param>
/// <param name="PageNumber">The requested page number (1-based).</param>
/// <param name="PageSize">The number of items to return per page.</param>
/// <param name="SortBy">The property name to sort by (optional).</param>
/// <param name="SortDirection">The direction of the sort (Ascending or Descending).</param>
/// <remarks>
/// This query returns a <see cref="PagedResult{SupportsResponse}"/>.
/// </remarks>
public record GetAllSupportsQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<SupportsResponse>>;


