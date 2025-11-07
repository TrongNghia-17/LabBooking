using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;

public class GetAllEquipmentsQueryHandler(
    ILogger<GetAllEquipmentsQueryHandler> logger,
    IEquipmentRepository equipmentRepository,
    IMapper mapper) : IRequestHandler<GetAllEquipmentsQuery, PagedResult<EquipmentResponse>>
{
    public async Task<PagedResult<EquipmentResponse>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all equipments");

        // Gọi phương thức repository
        var (equipments, totalCount) = await equipmentRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        // Ánh xạ kết quả sang DTO
        var equipmentsResponse = mapper.Map<IEnumerable<EquipmentResponse>>(equipments);

        // Đóng gói kết quả
        var result = new PagedResult<EquipmentResponse>(
            equipmentsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
