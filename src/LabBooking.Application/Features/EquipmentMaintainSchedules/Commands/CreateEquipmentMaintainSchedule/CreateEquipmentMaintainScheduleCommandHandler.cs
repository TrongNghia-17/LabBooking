using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandHandler(
    IMapper mapper,
    ILogger<CreateEquipmentMaintainScheduleCommandHandler> logger,
    IMediator mediator,
    ICurrentUserService currentUserService,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository)
    : IRequestHandler<CreateEquipmentMaintainScheduleCommand, EquipmentMaintainBatchResponse>
{
    public async Task<EquipmentMaintainBatchResponse> Handle(CreateEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        logger.LogInformation("User {UserId} bắt đầu tạo lịch bảo trì cho {Count} thiết bị.", currentUserId, request.EquipmentIds.Count);

        // 2. Tạo Object CHA (Header)
        var schedule = mapper.Map<EquipmentMaintainSchedule>(request);
        schedule.StartTime = schedule.StartTime.ToUniversalTime();
        schedule.EndTime = schedule.EndTime.ToUniversalTime();
        schedule.Id = Guid.NewGuid();
        schedule.Status = MaintenanceStatus.NotYet;

        var equipmentMap = new Dictionary<Guid, Equipment>();

        // 3. Loop tạo Object CON (Detail)
        foreach (var eqId in request.EquipmentIds)
        {
            var equipment = await equipmentRepository.GetByIdAsync(eqId)
                ?? throw new NotFoundException(nameof(Equipment), eqId.ToString());

            // Lưu vào map tạm
            equipmentMap[eqId] = equipment;

            // Check quyền Manager
            var labRoom = await labRoomRepository.GetByIdAsync(equipment.LabRoomId, cancellationToken);
            if (labRoom?.MainManagerId != currentUserId)
            {
                logger.LogWarning("Security: User {UserId} cố gắng bảo trì thiết bị {EqId} nhưng không có quyền.", currentUserId, eqId);
                throw new ForbidException($"Không có quyền với thiết bị {equipment.EquipmentName}.");
            }

            // Tạo dòng chi tiết
            var maintenance = new EquipmentMaintenance
            {
                Id = Guid.NewGuid(),
                EquipmentId = eqId,
                Status = MaintenanceStatus.NotYet,
            };

            schedule.Details.Add(maintenance);
        }

        // 4. LƯU DB (Lúc này Details chỉ chứa EquipmentId, nên EF Core sẽ lưu đúng)
        await equipmentMaintainScheduleRepository.CreateAsync(schedule, cancellationToken);

        // 5. GÁN LẠI EQUIPMENT ĐỂ MAPPER HOẠT ĐỘNG
        foreach (var detail in schedule.Details)
            if (equipmentMap.TryGetValue(detail.EquipmentId, out var eq))
                detail.Equipment = eq;

        logger.LogInformation("Tạo thành công Lịch bảo trì {ScheduleId}.", schedule.Id);

        if (schedule.StartTime <= DateTime.UtcNow.AddMinutes(5))
            await mediator.Send(new AutoUpdateEquipmentStatusJobCommand(), cancellationToken);

        // 6. Trả về (Lúc này detail.Equipment đã có dữ liệu nên Mapper sẽ lấy được Name)
        return mapper.Map<EquipmentMaintainBatchResponse>(schedule);
    }
}
