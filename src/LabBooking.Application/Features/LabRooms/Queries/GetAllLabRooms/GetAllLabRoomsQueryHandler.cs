using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAllLabRooms;

public class GetAllLabRoomsQueryHandler(
    ILogger<GetAllLabRoomsQueryHandler> logger,
    ILabRoomRepository labRoomRepository,
    IMapper mapper) : IRequestHandler<GetAllLabRoomsQuery, PagedResult<LabRoomResponse>>
{
    public async Task<PagedResult<LabRoomResponse>> Handle(GetAllLabRoomsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all lab rooms");

        var (labRooms, totalCount) = await labRoomRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var labRoomsResponse = mapper.Map<IEnumerable<LabRoomResponse>>(labRooms);

        var result = new PagedResult<LabRoomResponse>(
            labRoomsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
