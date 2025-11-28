using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

public class GetEquipmentsByCategoryQueryHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper
    ) : IRequestHandler<GetEquipmentsByCategoryQuery, IEnumerable<EquipmentSimpleResponse>>
{
    public async Task<IEnumerable<EquipmentSimpleResponse>> Handle(GetEquipmentsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var equipments = await repository.GetEquipmentsByCategoryIdAsync(request.CategoryId, cancellationToken);

        return mapper.Map<IEnumerable<EquipmentSimpleResponse>>(equipments);
    }

    private static string GetStatusVN(EquipmentStatus status) => status switch
    {
        EquipmentStatus.Available => "Sẵn sàng",
        EquipmentStatus.Maintain => "Đang bảo trì",
        EquipmentStatus.Broken => "Hỏng",
        _ => "Khác"
    };
}
