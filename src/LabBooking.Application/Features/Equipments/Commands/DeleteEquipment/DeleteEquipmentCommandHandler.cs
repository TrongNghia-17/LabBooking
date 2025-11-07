namespace LabBooking.Application.Features.Equipments.Commands.DeleteEquipment;

public class DeleteEquipmentCommandHandler(
    ILogger<DeleteEquipmentCommandHandler> logger,
    IEquipmentRepository equipmentRepository
    ) : IRequestHandler<DeleteEquipmentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting equipment with Id: {Id}", request.Id);

        var equipmentToDelete = await equipmentRepository.GetByIdAsync(request.Id);

        if (equipmentToDelete == null)
        {
            logger.LogWarning("Equipment with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Equipment), request.Id.ToString());
        }

        await equipmentRepository.DeleteAsync(equipmentToDelete);

        return Unit.Value;
    }
}
