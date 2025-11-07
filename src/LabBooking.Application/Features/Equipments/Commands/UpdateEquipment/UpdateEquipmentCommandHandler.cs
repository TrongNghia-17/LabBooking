namespace LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

public class UpdateEquipmentCommandHandler(
    ILogger<UpdateEquipmentCommandHandler> logger,
    IMapper mapper,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<UpdateEquipmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating equipment with Id: {Id}", request.Id);

        var equipmentToUpdate = await equipmentRepository.GetByIdAsync(request.Id);

        if (equipmentToUpdate == null)
        {
            logger.LogWarning("Equipment with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Equipment), request.Id.ToString());
        }

        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId);
        if (labRoom == null)
        {
            logger.LogWarning("LabRoom with ID: {LabRoomId} was not found.", request.LabRoomId);
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());
        }

        logger.LogInformation("LabRoom found. Update equipment.");
        mapper.Map(request, equipmentToUpdate);

        await equipmentRepository.Update(equipmentToUpdate);

        return Unit.Value;
    }
}
