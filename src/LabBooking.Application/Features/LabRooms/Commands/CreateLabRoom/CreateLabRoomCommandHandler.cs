namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

public class CreateLabRoomCommandHandler(
    ILogger<CreateLabRoomCommandHandler> logger,
    IMapper mapper,
    ILabRoomRepository labRoomRepository // Sử dụng repository của LabRoom
    ) : IRequestHandler<CreateLabRoomCommand, Guid>
{
    public async Task<Guid> Handle(CreateLabRoomCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new lab room");

        var labRoom = mapper.Map<LabRoom>(request);

        var labRoomId = await labRoomRepository.Create(labRoom);

        return labRoomId;
    }
}
