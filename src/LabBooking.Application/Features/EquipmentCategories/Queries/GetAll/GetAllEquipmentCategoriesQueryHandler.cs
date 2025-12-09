using LabBooking.Application.Features.EquipmentCategories.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public class GetAllEquipmentCategoriesQueryHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper,
    ICurrentUserService currentUserService, // Inject service để lấy User ID và Role
    ILogger<GetAllEquipmentCategoriesQueryHandler> logger
    ) : IRequestHandler<GetAllEquipmentCategoriesQuery, PagedResult<EquipmentCategoryResponse>>
{
    public async Task<PagedResult<EquipmentCategoryResponse>> Handle(GetAllEquipmentCategoriesQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin User hiện tại
        var userId = currentUserService.UserId;
        var userRoles = currentUserService.Roles; // Giả định service có thuộc tính Roles

        // Kiểm tra xem có phải Admin không?
        bool isAdmin = userRoles.Contains("Admin");

        logger.LogInformation("Getting Equipment Categories. User: {UserId}, IsAdmin: {IsAdmin}", userId, isAdmin);

        // 2. Gọi Repository với thông tin User
        var (categories, totalCount) = await repository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            userId,      // Truyền ID
            isAdmin,     // Truyền quyền
            cancellationToken);

        // 3. Map sang DTO
        var dtos = mapper.Map<IEnumerable<EquipmentCategoryResponse>>(categories);

        // 4. Trả về kết quả
        return new PagedResult<EquipmentCategoryResponse>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}
