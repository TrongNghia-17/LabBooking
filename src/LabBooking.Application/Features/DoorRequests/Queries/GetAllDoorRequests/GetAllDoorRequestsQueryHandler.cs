namespace LabBooking.Application.Features.DoorRequests.Queries.GetAllDoorRequests;

public class GetAllDoorRequestsQueryHandler(
    ILogger<GetAllDoorRequestsQueryHandler> logger,
    IDoorRequestRepository doorRequestRepository,
    IMapper mapper
    ) : IRequestHandler<GetAllDoorRequestsQuery, PagedResult<DoorRequestsResponse>>
{
    public async Task<PagedResult<DoorRequestsResponse>> Handle(GetAllDoorRequestsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all door requests");
        var (doorRequests, totalCount) = await doorRequestRepository.GetAllMatchingAsync(
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.StatusFilter);

        var doorRequestsResponse = mapper.Map<IEnumerable<DoorRequestsResponse>>(doorRequests);

        var result = new PagedResult<DoorRequestsResponse>(
            doorRequestsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);
        return result;
    }
}
