using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

public class GetEquipmentsByCategoryQueryHandler(
    IEquipmentCategoryRepository repository,
    ICurrentUserService currentUserService,
    ILogger<GetEquipmentsByCategoryQueryHandler> logger,
    IMapper mapper
    ) : IRequestHandler<GetEquipmentsByCategoryQuery, IEnumerable<EquipmentSimpleResponse>>
{
    public async Task<IEnumerable<EquipmentSimpleResponse>> Handle(GetEquipmentsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var isExist = await repository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!isExist)
            throw new NotFoundException(nameof(EquipmentCategory), request.CategoryId.ToString());

        IEnumerable<Equipment> equipments;
        var userId = currentUserService.UserId;
        var roles = currentUserService.Roles.ToList();

        logger.LogInformation("GetEquipmentsByCategory: UserID={UserId}, Roles=[{Roles}], CategoryId={CatId}",
            userId, string.Join(",", roles), request.CategoryId);

        bool isManager = roles.Contains("Manager");
        bool isAdmin = roles.Contains("Admin");

        if (isManager && !isAdmin)
        {
            if (userId == null)
            {
                logger.LogError("Security Alert: User có Role Manager nhưng không tìm thấy UserID (Token lỗi?).");
                throw new UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            logger.LogInformation("User là Manager -> Lọc thiết bị theo phòng quản lý.");

            equipments = await repository.GetEquipmentsByCategoryAndManagerAsync(
                request.CategoryId,
                userId.Value,
                cancellationToken);
        }
        else
        {
            logger.LogInformation("User là Admin/Student/Khác -> Lấy toàn bộ thiết bị trong Category.");

            equipments = await repository.GetEquipmentsByCategoryIdAsync(
                request.CategoryId,
                cancellationToken);
        }

        logger.LogInformation("Tìm thấy {Count} thiết bị.", equipments.Count());

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
