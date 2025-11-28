using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandHandler(
    ILogger<CreateEquipmentMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateEquipmentMaintainScheduleCommand, EquipmentMaintainScheduleResponse>
{
    public async Task<EquipmentMaintainScheduleResponse> Handle(CreateEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

        var equipment = await equipmentRepository.GetByIdAsync(request.EquipmentId)
            ?? throw new NotFoundException(nameof(Equipment), request.EquipmentId.ToString());

        var labRoom = await labRoomRepository.GetByIdAsync(equipment.LabRoomId, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), equipment.LabRoomId.ToString());

        if (labRoom.MainManagerId != currentUserId)
        {
            logger.LogWarning("User {UserId} cố gắng bảo trì thiết bị {EqId} thuộc Lab {LabId} nhưng không phải quản lý.", currentUserId, request.EquipmentId, labRoom.Id);
            throw new ForbidException("Bạn không có quyền bảo trì thiết bị này vì nó thuộc phòng Lab bạn không quản lý.");
        }

        logger.LogInformation("Đang tạo lịch bảo trì cho thiết bị {EqId} trong Lab {LabId}", request.EquipmentId, labRoom.Id);

        var schedule = mapper.Map<EquipmentMaintainSchedule>(request);
        schedule.Id = Guid.NewGuid();
        schedule.EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet;

        await equipmentMaintainScheduleRepository.Create(schedule, cancellationToken);

        if (request.StartTime <= DateTime.UtcNow.AddMinutes(5))
        {
            if (equipment.Status != EquipmentStatus.Maintain)
            {
                equipment.Status = EquipmentStatus.Maintain;
                equipment.IsAvailable = false;

                await equipmentRepository.Update(equipment);
                logger.LogInformation("Đã tự động cập nhật trạng thái thiết bị {EqId} sang 'Maintain' vì lịch bảo trì bắt đầu ngay.", equipment.Id);
            }
        }

        return mapper.Map<EquipmentMaintainScheduleResponse>(schedule);
    }
}