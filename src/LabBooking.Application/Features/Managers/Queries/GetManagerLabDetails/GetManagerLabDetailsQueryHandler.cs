using LabBooking.Application.Features.Managers.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Managers.Queries.GetManagerLabDetails;

public class GetManagerLabDetailsQueryHandler(
    ICurrentUserService currentUserService,
    UserManager<User> userManager,
    IMapper mapper,
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

        var labDtos = mapper.Map<List<ManagerLabRoomDto>>(managedLabs);

        return new ManagerLabDetailsResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ManagedLabs = labDtos
        };
    }
}
