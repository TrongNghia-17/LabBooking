namespace LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

public class AutoUpdateEquipmentStatusJobHandler(
    IEquipmentMaintainScheduleRepository repository,
    ILogger<AutoUpdateEquipmentStatusJobHandler> logger
    ) : IRequestHandler<AutoUpdateEquipmentStatusJobCommand, string>
{
    public async Task<string> Handle(AutoUpdateEquipmentStatusJobCommand request, CancellationToken cancellationToken)
    {
        // Lấy giờ hiện tại UTC
        var utcNow = DateTime.UtcNow;
        // Tạo giờ Việt Nam để log cho dễ nhìn (UTC + 7)
        var vnTime = utcNow.AddHours(7);

        logger.LogInformation("CronJob [AutoUpdateStatus]: Bắt đầu quét lúc {Time} (VN)...", vnTime);

        try
        {
            // Gọi logic nghiệp vụ từ Repo
            var resultMessage = await repository.ProcessAutomatedMaintenanceAsync(cancellationToken);

            var finalMessage = $"{resultMessage} | Thời gian quét: {vnTime:HH:mm:ss dd/MM/yyyy}";

            // Log kết quả
            logger.LogInformation("CronJob [AutoUpdateStatus]: Success - {Message}", finalMessage);
            return finalMessage;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CronJob [AutoUpdateStatus]: Thất bại.");
            throw; // Ném lỗi để hệ thống Job biết mà retry (nếu có cấu hình)
        }
    }
}
