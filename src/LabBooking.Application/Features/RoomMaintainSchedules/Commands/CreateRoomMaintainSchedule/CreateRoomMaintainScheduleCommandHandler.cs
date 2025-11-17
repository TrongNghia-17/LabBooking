namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;

public class CreateRoomMaintainScheduleCommandHandler(
    ILogger<CreateRoomMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository // Repository mới
    ) : IRequestHandler<CreateRoomMaintainScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang tạo một RoomMaintainSchedule mới cho LabRoom {LabRoomId}", request.LabRoomId);

        // 1. Map từ Command sang Entity
        var schedule = mapper.Map<RoomMaintainSchedule>(request);

        // 2. Gán các giá trị mặc định
        // Entity RoomMaintainSchedule không tự gán Id, 
        // không giống như UsagePolicy. Vì vậy, chúng ta gán nó ở đây.
        schedule.Id = Guid.NewGuid();
        schedule.RoomMaintainStatus = RoomMaintainStatus.NotYet; // Gán trạng thái mặc định

        // 3. Lưu vào database
        // (Giả định phương thức Create trả về Guid giống như LabRoomRepository)
        var scheduleId = await roomMaintainScheduleRepository.Create(schedule, cancellationToken);

        return scheduleId;
    }
}