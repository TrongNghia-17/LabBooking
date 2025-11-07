using LabBooking.Domain.Repositories;

namespace LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommandHandler(
    ILogger<CreateEquipmentCommandHandler> logger,
    IMapper mapper,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<CreateEquipmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CreateEquipmentCommand for LabRoomId: {LabRoomId}", request.LabRoomId);

        // 1. Kiểm tra sự tồn tại của LabRoom
        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId);
        if (labRoom == null)
        {
            logger.LogWarning("LabRoom with ID: {LabRoomId} was not found.", request.LabRoomId);
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());
        }

        // 2. Nếu LabRoom tồn tại, tiếp tục logic tạo Equipment
        logger.LogInformation("LabRoom found. Creating new equipment.");
        var equipment = mapper.Map<Equipment>(request);

        var equipmentId = await equipmentRepository.Create(equipment);

        return equipmentId;
    }
}
