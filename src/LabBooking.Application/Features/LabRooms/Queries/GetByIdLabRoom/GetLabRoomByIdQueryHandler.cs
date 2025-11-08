using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetByIdLabRoom;

public class GetLabRoomByIdQueryHandler(
    ILogger<GetLabRoomByIdQueryHandler> logger,
    ILabRoomRepository labRoomRepository,
    IMapper mapper) : IRequestHandler<GetLabRoomByIdQuery, LabRoomResponse>
{
    public async Task<LabRoomResponse> Handle(GetLabRoomByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting LabRoom by Id: {LabRoomId}", request.Id);

        var labRoom = await labRoomRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), request.Id.ToString());

        var labRoomResponse = mapper.Map<LabRoomResponse>(labRoom);

        return labRoomResponse;
    }
}
