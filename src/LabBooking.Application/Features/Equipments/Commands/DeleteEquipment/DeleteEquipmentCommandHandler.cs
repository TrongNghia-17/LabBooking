namespace LabBooking.Application.Features.Equipments.Commands.DeleteEquipment;

/// <summary>
/// Handles the <see cref="DeleteEquipmentCommand"/>.
/// </summary>
public class DeleteEquipmentCommandHandler(
    ILogger<DeleteEquipmentCommandHandler> logger,
    IEquipmentRepository equipmentRepository
    ) : IRequestHandler<DeleteEquipmentCommand, Unit>
{
    /// <summary>
    /// Handles the logic to find and delete an equipment.
    /// </summary>
    /// <param name="request">The command containing the equipment ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Unit"/> value indicating completion.</returns>
    /// <exception cref="NotFoundException">Thrown if the equipment with the specified ID is not found.</exception>
    public async Task<Unit> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing request to delete equipment with Id: {EquipmentId}", request.Id);

        var equipmentToDelete = await equipmentRepository.GetByIdAsync(request.Id);

        if (equipmentToDelete == null)
        {
            logger.LogWarning("Equipment with Id: {EquipmentId} not found.", request.Id);
            throw new NotFoundException(nameof(Equipment), request.Id.ToString());
        }

        await equipmentRepository.DeleteAsync(equipmentToDelete);

        logger.LogInformation("Successfully deleted equipment with Id: {EquipmentId}", request.Id);

        return Unit.Value;
    }
}
