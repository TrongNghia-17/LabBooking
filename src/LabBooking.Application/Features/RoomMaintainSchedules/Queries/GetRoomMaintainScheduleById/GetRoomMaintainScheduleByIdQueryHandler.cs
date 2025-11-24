using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetRoomMaintainScheduleById;

public class GetRoomMaintainScheduleByIdQueryHandler(
    ILogger<GetRoomMaintainScheduleByIdQueryHandler> logger,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    IMapper mapper
    ) : IRequestHandler<GetRoomMaintainScheduleByIdQuery, RoomMaintainScheduleResponse>
{
    public async Task<RoomMaintainScheduleResponse> Handle(GetRoomMaintainScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy RoomMaintainSchedule bằng Id: {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var schedule = await roomMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(RoomMaintainSchedule), request.Id.ToString());

        // 2. Map entity sang DTO
        var scheduleResponse = mapper.Map<RoomMaintainScheduleResponse>(schedule);

        return scheduleResponse;
    }
}
