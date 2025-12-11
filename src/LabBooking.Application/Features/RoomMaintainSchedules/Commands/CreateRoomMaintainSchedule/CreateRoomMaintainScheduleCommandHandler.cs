using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;

public class CreateRoomMaintainScheduleCommandHandler(
    ILogger<CreateRoomMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService// Repository mới
    ) : IRequestHandler<CreateRoomMaintainScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");
        }

        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);

        if (labRoom == null)
        {
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());
        }

        if (labRoom.MainManagerId != currentUserId)
        {
            logger.LogWarning("User {UserId} cố gắng tạo lịch bảo trì cho Lab {LabId} nhưng không phải là quản lý.", currentUserId, request.LabRoomId);
            throw new ForbidException("Bạn không có quyền tạo lịch bảo trì cho phòng Lab này vì bạn không phải là người quản lý nó.");
        }

        logger.LogInformation("Đang tạo một RoomMaintainSchedule mới cho LabRoom {LabRoomId} bởi Manager {ManagerId}", request.LabRoomId, currentUserId);

        // 1. Map từ Command sang Entity
        var schedule = mapper.Map<RoomMaintainSchedule>(request);

        // 2. Gán các giá trị mặc định
        // Entity RoomMaintainSchedule không tự gán Id, 
        // không giống như UsagePolicy. Vì vậy, chúng ta gán nó ở đây.
        schedule.Id = Guid.NewGuid();
        schedule.RoomMaintainStatus = RoomMaintainStatus.NotYet; // Gán trạng thái mặc định

        // 3. Lưu vào database
        // (Giả định phương thức Create trả về Guid giống như LabRoomRepository)
        //var scheduleId = await roomMaintainScheduleRepository.Create(schedule, cancellationToken);
        var scheduleId = await roomMaintainScheduleRepository.CreateWithOverrideLogicAsync(schedule, cancellationToken);
        return scheduleId;
    }
}