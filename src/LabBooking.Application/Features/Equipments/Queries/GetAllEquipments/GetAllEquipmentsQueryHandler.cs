using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;

/// <summary>
/// Handles the <see cref="GetAllEquipmentsQuery"/>.
/// </summary>
public class GetAllEquipmentsQueryHandler(
    ILogger<GetAllEquipmentsQueryHandler> logger,
    IEquipmentRepository equipmentRepository,
    IMapper mapper) : IRequestHandler<GetAllEquipmentsQuery, PagedResult<EquipmentResponse>>
{
    /// <summary>
    /// Handles the logic to retrieve and map a paginated list of equipments.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged result of <see cref="EquipmentResponse"/>.</returns>
    public async Task<PagedResult<EquipmentResponse>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processing query to get all equipments. Page: {PageNumber}, Size: {PageSize}, SortBy: {SortBy}, Search: {SearchPhrase}",
            request.PageNumber, request.PageSize, request.SortBy, request.SearchPhrase);

        var (equipments, totalCount) = await equipmentRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var equipmentsResponse = mapper.Map<IEnumerable<EquipmentResponse>>(equipments);

        var result = new PagedResult<EquipmentResponse>(
            equipmentsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
