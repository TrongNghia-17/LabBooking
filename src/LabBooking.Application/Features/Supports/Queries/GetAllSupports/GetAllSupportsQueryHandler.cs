using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetAllSupports;

/// <summary>
/// Handles the <see cref="GetAllSupportsQuery"/> to retrieve support tickets.
/// </summary>
/// <remarks>
/// This handler fetches data from the repository based on pagination, filtering, and sorting parameters,
/// maps the entities to response DTOs, and returns a paginated result.
/// </remarks>
public class GetAllSupportsQueryHandler(
    ILogger<GetAllSupportsQueryHandler> logger,
    ISupportRepository supportRepository,
    IMapper mapper) : IRequestHandler<GetAllSupportsQuery, PagedResult<SupportsResponse>>
{
    /// <summary>
    /// Handles the incoming query request.
    /// </summary>
    /// <param name="request">The query request containing pagination, sorting, and filtering options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="PagedResult{SupportsResponse}"/> containing the list of supports and pagination metadata.</returns>
    public async Task<PagedResult<SupportsResponse>> Handle(GetAllSupportsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processing GetAllSupportsQuery: PageSize={PageSize}, PageNumber={PageNumber}, SortBy={SortBy}, SortDirection={SortDirection}, SearchPhrase={SearchPhrase}",
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.SearchPhrase);

        var (supports, totalCount) = await supportRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var supportsResponse = mapper.Map<IEnumerable<SupportsResponse>>(supports);

        var result = new PagedResult<SupportsResponse>(
            supportsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
