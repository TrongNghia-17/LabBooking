namespace LabBooking.Application.Features.DoorRequests.Queries.GetAllDoorRequests;

public record GetAllDoorRequestsQuery(
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection,
    DoorRequestStatus? StatusFilter
    ) : IRequest<PagedResult<DoorRequestsResponse>>;
