using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public record GetAllEquipmentCategoriesQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<EquipmentCategoryResponse>>;
