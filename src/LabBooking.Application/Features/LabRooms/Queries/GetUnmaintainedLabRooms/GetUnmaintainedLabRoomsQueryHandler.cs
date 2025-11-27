using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetUnmaintainedLabRooms;

public class GetUnmaintainedLabRoomsQueryHandler(
    ILogger<GetUnmaintainedLabRoomsQueryHandler> logger,
    ILabRoomRepository labRoomRepository,
    IMapper mapper) : IRequestHandler<GetUnmaintainedLabRoomsQuery, IEnumerable<LabRoomResponse>>
{
    public async Task<IEnumerable<LabRoomResponse>> Handle(GetUnmaintainedLabRoomsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all lab rooms with pending maintenance status (NotYet)");

        // Gọi phương thức mới từ Repository
        var unmaintainedLabRooms = await labRoomRepository.GetUnmaintainedLabRoomsAsync(cancellationToken);

        // Map sang DTO
        var response = mapper.Map<IEnumerable<LabRoomResponse>>(unmaintainedLabRooms);

        return response;
    }
}
