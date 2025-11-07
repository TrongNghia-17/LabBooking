using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

public class CreateLabRoomCommandHandler(
    ILogger<CreateLabRoomCommandHandler> logger,
    IMapper mapper,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateLabRoomCommand, Guid>
{
    public async Task<Guid> Handle(CreateLabRoomCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUserService.UserId;

        if (creatorId == null)
        {
            logger.LogWarning("Không tìm thấy thông tin người dùng (chưa đăng nhập).");
            throw new UnauthorizedAccessException("Người dùng không được xác thực.");
        }

        logger.LogInformation("Người dùng {CreatorId} đang tạo phòng lab mới", creatorId.Value);

        var labRoom = mapper.Map<LabRoom>(request);

        var labRoomId = await labRoomRepository.Create(labRoom);

        return labRoomId;
    }
}
