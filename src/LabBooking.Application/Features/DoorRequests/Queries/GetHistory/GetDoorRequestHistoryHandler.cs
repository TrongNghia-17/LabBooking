using LabBooking.Application.Features.DoorRequests.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetHistory;

public class GetDoorRequestHistoryHandler(
    IDoorRequestRepository repo,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetDoorRequestHistoryQuery, IEnumerable<DoorRequestHistoryDto>>
{
    public async Task<IEnumerable<DoorRequestHistoryDto>> Handle(GetDoorRequestHistoryQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy User từ Token
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        var roles = currentUserService.Roles; // Giả sử service trả về List<string>

        // 2. Kiểm tra Quyền: Bảo vệ hoặc Admin được xem hết
        bool canViewAll = roles.Contains("SecurityGuard") || roles.Contains("Admin");

        // 3. Gọi Repository
        var entities = await repo.GetHistoryAsync(
            userId,
            canViewAll,
            request.LabRoomId,
            request.Status,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        // 4. Map sang DTO
        return mapper.Map<IEnumerable<DoorRequestHistoryDto>>(entities);
    }
}
