using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public class GetAllEquipmentCategoriesQueryHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper
    ) : IRequestHandler<GetAllEquipmentCategoriesQuery, IEnumerable<EquipmentCategoryResponse>>
{
    public async Task<IEnumerable<EquipmentCategoryResponse>> Handle(GetAllEquipmentCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllAsync(cancellationToken);

        return mapper.Map<IEnumerable<EquipmentCategoryResponse>>(categories);
    }
}
