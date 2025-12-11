using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAllLabRooms;

public record GetAllLabRoomsQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection,
    DateOnly? FilterDate,
    Guid? FilterSlotId
) : IRequest<PagedResult<LabRoomResponse>>;
