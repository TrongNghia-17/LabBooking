using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequests;

public record GetDoorRequestsQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection,
    DateOnly? FilterDate,
    DoorRequestStatus? FilterStatus
) : IRequest<PagedResult<DoorRequestDto>>;
