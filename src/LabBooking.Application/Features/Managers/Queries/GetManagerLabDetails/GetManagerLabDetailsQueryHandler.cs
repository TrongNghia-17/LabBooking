using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.Managers.Dtos;

namespace LabBooking.Application.Features.Managers.Queries.GetManagerLabDetails;

public class GetManagerLabDetailsQueryHandler(
    ICurrentUserService currentUserService,
    UserManager<User> userManager,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<GetManagerLabDetailsQuery, ManagerLabDetailsResponse>
{
    public async Task<ManagerLabDetailsResponse> Handle(GetManagerLabDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa xác thực (đăng nhập).");

        var user = await userManager.FindByIdAsync(currentUserId.ToString())
            ?? throw new NotFoundException("Người dùng", currentUserId.ToString());

        var managedLabs = await labRoomRepository.GetLabsByManagerWithEquipmentsAsync(currentUserId, cancellationToken);

        var labDtos = managedLabs.Select(lab => new ManagerLabRoomDto
        {
            Id = lab.Id,
            LabName = lab.LabName ?? "Chưa đặt tên",
            Location = lab.Location,
            MaximumLimit = lab.MaximumLimit,
            Status = lab.IsActive ? "Đang hoạt động" : "Ngừng hoạt động",

            EquipmentGroups = lab.Equipments?
                .GroupBy(e => e.EquipmentCategory?.Name ?? "Chưa phân loại")
                .Select(group => new EquipmentCategoryGroupDto
                {
                    CategoryName = group.Key,
                    TotalCount = group.Count(),

                    Items = group.Select(eq => new ManagerEquipmentDto
                    {
                        Id = eq.Id,
                        EquipmentName = eq.EquipmentName,
                        Description = eq.Description,
                        IsAvailable = eq.IsAvailable,
                        Status = GetStatusVN(eq.Status)
                    }).ToList()
                })
                .OrderBy(g => g.CategoryName)
                .ToList() ?? new List<EquipmentCategoryGroupDto>()
        }).ToList();

        return new ManagerLabDetailsResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ManagedLabs = labDtos
        };
    }

    private static string GetStatusVN(EquipmentStatus status) => status switch
    {
        EquipmentStatus.Available => "Sẵn sàng",
        EquipmentStatus.Maintain => "Đang bảo trì",
        EquipmentStatus.Broken => "Hỏng",
        _ => "Khác"
    };
}
