using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetAllSupports;

public class GetAllSupportsQueryHandler(
    ILogger<GetAllSupportsQueryHandler> logger,
    ISupportRepository supportRepository,
    IMapper mapper) : IRequestHandler<GetAllSupportsQuery, PagedResult<SupportsResponse>>
{
    public async Task<PagedResult<SupportsResponse>> Handle(GetAllSupportsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all supports");

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
