using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;

public class CreateRoomMaintainScheduleCommandHandler(
    ILogger<CreateRoomMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateRoomMaintainScheduleCommand, RoomMaintainScheduleResponse>
{
    public async Task<RoomMaintainScheduleResponse> Handle(CreateRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken) ?? throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());

        if (labRoom.MainManagerId != currentUserId)
        {
            logger.LogWarning("User {UserId} cố gắng tạo lịch bảo trì cho Lab {LabId} nhưng không phải là quản lý.", currentUserId, request.LabRoomId);
            throw new ForbidException("Bạn không có quyền tạo lịch bảo trì cho phòng Lab này vì bạn không phải là người quản lý nó.");
        }

        logger.LogInformation("Đang tạo một RoomMaintainSchedule mới cho LabRoom {LabRoomId} bởi Manager {ManagerId}", request.LabRoomId, currentUserId);

        var schedule = mapper.Map<RoomMaintainSchedule>(request);

        schedule.Id = Guid.NewGuid();
        schedule.RoomMaintainStatus = RoomMaintainStatus.NotYet;

        var createdSchedule = await roomMaintainScheduleRepository.Create(schedule, cancellationToken);
        var response = mapper.Map<RoomMaintainScheduleResponse>(createdSchedule);

        logger.LogInformation("Created Schedule {Id} for Lab {LabName}", createdSchedule.Id, createdSchedule.LabRoom?.LabName);

        return response;
    }
}