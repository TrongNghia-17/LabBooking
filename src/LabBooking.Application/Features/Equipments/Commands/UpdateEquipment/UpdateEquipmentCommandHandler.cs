namespace LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

/// <summary>
/// Handles the <see cref="UpdateEquipmentCommand"/>.
/// </summary>
public class UpdateEquipmentCommandHandler(
    ILogger<UpdateEquipmentCommandHandler> logger,
    IMapper mapper,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<UpdateEquipmentCommand, Unit>
{
    /// <summary>
    /// Handles the logic to validate and update an equipment entity.
    /// </summary>
    /// <param name="request">The command containing update details and the equipment ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Unit"/> value indicating completion.</returns>
    /// <exception cref="NotFoundException">Thrown if the equipment or the specified LabRoom is not found.</exception>
    public async Task<Unit> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing request to update equipment: {EquipmentId}", request.Id);

        var equipmentToUpdate = await equipmentRepository.GetByIdAsync(request.Id);

        if (equipmentToUpdate == null)
        {
            logger.LogWarning("Equipment with Id: {EquipmentId} not found.", request.Id);
            throw new NotFoundException(nameof(Equipment), request.Id.ToString());
        }

        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId);
        if (labRoom == null)
        {
            logger.LogWarning("LabRoom with ID: {LabRoomId} was not found. Update cancelled.", request.LabRoomId);
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());
        }

        mapper.Map(request, equipmentToUpdate);

        await equipmentRepository.Update(equipmentToUpdate);

        logger.LogInformation("Successfully updated equipment: {EquipmentId}", equipmentToUpdate.Id);

        return Unit.Value;
    }
}
