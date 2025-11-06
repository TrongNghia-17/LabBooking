namespace LabBooking.Application.Features.LabRooms.Commands.UpdateLabRoom;

public class UpdateLabRoomCommandHandler(
    ILogger<UpdateLabRoomCommandHandler> logger,
    IMapper mapper,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<UpdateLabRoomCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLabRoomCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating lab room with Id: {Id}", request.Id);

        var labRoomToUpdate = await labRoomRepository.GetByIdAsync(request.Id);

        if (labRoomToUpdate == null)
        {
            logger.LogWarning("Lab room with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(LabRoom), request.Id.ToString());
        }

        mapper.Map(request, labRoomToUpdate);

        labRoomToUpdate.LastUpdatedDate = DateTime.UtcNow;

        await labRoomRepository.Update(labRoomToUpdate);

        return Unit.Value;
    }
}
