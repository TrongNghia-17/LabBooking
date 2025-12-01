using LabBooking.Application.Features.EquipmentCategories.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public class GetAllEquipmentCategoriesQueryHandler(
    IEquipmentCategoryRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetAllEquipmentCategoriesQuery, IEnumerable<EquipmentCategoryResponse>>
{
    public async Task<IEnumerable<EquipmentCategoryResponse>> Handle(GetAllEquipmentCategoriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        var categories = await repository.GetByManagerIdAsync(userId, cancellationToken);

        return mapper.Map<IEnumerable<EquipmentCategoryResponse>>(categories);
    }
}
