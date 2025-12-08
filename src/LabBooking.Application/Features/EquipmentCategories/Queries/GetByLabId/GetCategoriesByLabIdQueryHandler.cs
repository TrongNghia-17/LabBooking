using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetByLabId;

public class GetCategoriesByLabIdQueryHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper
    ) : IRequestHandler<GetCategoriesByLabIdQuery, IEnumerable<EquipmentCategoryResponse>>
{
    public async Task<IEnumerable<EquipmentCategoryResponse>> Handle(GetCategoriesByLabIdQuery request, CancellationToken cancellationToken)
    {
        // Gọi repository để lấy category và thiết bị theo LabId
        var categories = await repository.GetByLabIdAsync(request.LabId, cancellationToken);

        // Map sang DTO Response
        // Lưu ý: EquipmentCategoryResponse cần được cấu hình Map để chứa cả list Equipment
        return mapper.Map<IEnumerable<EquipmentCategoryResponse>>(categories);
    }
}
