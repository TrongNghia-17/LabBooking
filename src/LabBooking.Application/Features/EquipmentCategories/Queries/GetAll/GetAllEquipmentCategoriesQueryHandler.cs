using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public class GetAllEquipmentCategoriesQueryHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper,
    ILogger<GetAllEquipmentCategoriesQueryHandler> logger
    ) : IRequestHandler<GetAllEquipmentCategoriesQuery, PagedResult<EquipmentCategoryResponse>>
{
    public async Task<PagedResult<EquipmentCategoryResponse>> Handle(GetAllEquipmentCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Equipment Categories: Page {Page}, Size {Size}, Search '{Search}'",
            request.PageNumber, request.PageSize, request.SearchPhrase);

        // Gọi Repository
        var (categories, totalCount) = await repository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        // Map sang DTO
        var dtos = mapper.Map<IEnumerable<EquipmentCategoryResponse>>(categories);

        // Trả về kết quả phân trang
        return new PagedResult<EquipmentCategoryResponse>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}
