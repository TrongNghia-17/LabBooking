namespace LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

public class AutoUpdateEquipmentStatusJobHandler(
    IEquipmentMaintainScheduleRepository repository,
    INotificationRepository notificationRepository,
    ILogger<AutoUpdateEquipmentStatusJobHandler> logger
    ) : IRequestHandler<AutoUpdateEquipmentStatusJobCommand, string>
{
    public async Task<string> Handle(AutoUpdateEquipmentStatusJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Chạy logic bảo trì (Giữ nguyên không đụng vào)
            var resultMessage = await repository.ProcessAutomatedMaintenanceAsync(cancellationToken);

            // 2. Lấy danh sách vừa hoàn tất để thông báo
            var finishedSchedules = await repository.GetRecentlyFinishedSchedulesAsync(10, cancellationToken);

            if (finishedSchedules.Any())
            {
                var pushQueue = new List<PushNotificationData>();

                foreach (var schedule in finishedSchedules)
                {
                    // Lấy danh sách mô tả thiết bị (Dùng Description vì Entity Equipment không có Name)
                    var deviceNames = string.Join(", ", schedule.Details
                        .Where(d => d.Equipment != null)
                        .Select(d => d.Equipment.Description));

                    if (schedule.CreatedBy.HasValue)
                    {
                        var (notification, pushData) = notificationRepository.PrepareNotification(
                            schedule.CreatedBy.Value,
                            "🛠️ Bảo trì hoàn tất",
                            $"Lịch bảo trì '{schedule.Description}' đã xong. Thiết bị: [{deviceNames}]",
                            "MAINTENANCE_COMPLETED",
                            new { scheduleId = schedule.Id }
                        );

                        // SỬA LỖI: Dùng CreateAsync thay vì AddAsync
                        await notificationRepository.CreateAsync(notification, cancellationToken);

                        pushQueue.Add(pushData);
                    }
                }

                if (pushQueue.Any())
                {
                    notificationRepository.RunPushNotificationTask(pushQueue);
                }
            }

            return resultMessage;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi chạy Job cập nhật bảo trì");
            throw;
        }
    }
}
