using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;

/// <summary>
/// Represents the query to get a paged list of all equipments,
/// with optional filtering and sorting.
/// </summary>
public record GetAllEquipmentsQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<EquipmentResponse>>;
