using LabBooking.Application.Features.Managers.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Managers.Queries.GetManagerProfile;

public class GetManagerProfileQueryHandler(
    ICurrentUserService currentUserService,
    UserManager<User> userManager,
    ILabRoomRepository labRoomRepository
    ) : IRequestHandler<GetManagerProfileQuery, ManagerProfileResponse>
{
    public async Task<ManagerProfileResponse> Handle(GetManagerProfileQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy UserId từ Token (ICurrentUserService)
        var currentUserId = currentUserService.UserId;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        // 2. Lấy thông tin User từ Identity
        var user = await userManager.FindByIdAsync(currentUserId.Value.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(User), currentUserId.Value.ToString());
        }

        // 3. Lấy danh sách LabRoom do user này quản lý
        var managedLabs = await labRoomRepository.GetByManagerIdAsync(currentUserId.Value, cancellationToken);

        // 4. Map sang DTO
        var labDtos = managedLabs.Select(l => new ManagedLabRoomDto(
            l.Id,
            l.LabName ?? "Unnamed Lab",
            l.Location ?? "Unknown"
        )).ToList();

        return new ManagerProfileResponse(
            user.Id,
            user.UserName ?? "", // Hoặc user.FullName nếu bạn đã thêm trường này
            user.Email ?? "",
            labDtos
        );
    }
}
