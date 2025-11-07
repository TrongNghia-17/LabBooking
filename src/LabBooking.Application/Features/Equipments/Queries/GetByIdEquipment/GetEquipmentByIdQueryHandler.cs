using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetByIdEquipment;

public class GetEquipmentByIdQueryHandler(
    ILogger<GetEquipmentByIdQueryHandler> logger,
    IEquipmentRepository equipmentRepository,
    IMapper mapper) : IRequestHandler<GetEquipmentByIdQuery, EquipmentResponse>
{
    public async Task<EquipmentResponse> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Equipment by Id: {EquipmentId}", request.Id);

        var equipment = await equipmentRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Equipment), request.Id.ToString());

        var equipmentResponse = mapper.Map<EquipmentResponse>(equipment);

        return equipmentResponse;
    }
}
