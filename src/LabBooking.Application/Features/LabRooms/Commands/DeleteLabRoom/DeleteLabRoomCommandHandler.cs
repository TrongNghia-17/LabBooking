namespace LabBooking.Application.Features.LabRooms.Commands.DeleteLabRoom;

public class DeleteLabRoomCommandHandler(
    ILogger<DeleteLabRoomCommandHandler> logger,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<DeleteLabRoomCommand, Unit>
{
    public async Task<Unit> Handle(DeleteLabRoomCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting lab room with Id: {Id}", request.Id);

        var labRoomToDelete = await labRoomRepository.GetByIdAsync(request.Id);

        if (labRoomToDelete == null)
        {
            logger.LogWarning("Lab room with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(LabRoom), request.Id.ToString());
        }

        await labRoomRepository.DeleteAsync(labRoomToDelete, cancellationToken);

        return Unit.Value;
    }
}
