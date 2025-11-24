using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetByIdEquipment;

/// <summary>
/// Handles the <see cref="GetEquipmentByIdQuery"/>.
/// </summary>
public class GetEquipmentByIdQueryHandler(
    ILogger<GetEquipmentByIdQueryHandler> logger,
    IEquipmentRepository equipmentRepository,
    IMapper mapper) : IRequestHandler<GetEquipmentByIdQuery, EquipmentResponse>
{
    /// <summary>
    /// Handles the logic to find and return an equipment DTO.
    /// </summary>
    /// <param name="request">The query containing the equipment ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="EquipmentResponse"/> object.</returns>
    /// <exception cref="NotFoundException">Thrown if the equipment is not found.</exception>
    public async Task<EquipmentResponse> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing request to get equipment by Id: {EquipmentId}", request.Id);

        var equipment = await equipmentRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Equipment), request.Id.ToString());

        logger.LogInformation("Successfully retrieved equipment: {EquipmentId}", equipment.Id);
        var equipmentResponse = mapper.Map<EquipmentResponse>(equipment);

        return equipmentResponse;
    }
}
