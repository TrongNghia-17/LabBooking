namespace LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

public class AutoUpdateEquipmentStatusJobHandler(
    IEquipmentMaintainScheduleRepository repository,
    ILogger<AutoUpdateEquipmentStatusJobHandler> logger
    ) : IRequestHandler<AutoUpdateEquipmentStatusJobCommand, string>
{
    public async Task<string> Handle(AutoUpdateEquipmentStatusJobCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var vnTime = utcNow.AddHours(7);

        // 1. Dùng ký tự đặc biệt để tách biệt Job này với các log khác
        logger.LogInformation("===============================================================");
        logger.LogInformation(">>> [CRON-START] AutoUpdateStatus | VN Time: {Time:HH:mm:ss dd/MM}", vnTime);

        try
        {
            var resultMessage = await repository.ProcessAutomatedMaintenanceAsync(cancellationToken);

            // 2. Format lại message cho gọn
            var finalMessage = $"Result: {resultMessage}";

            logger.LogInformation("<<< [CRON-END]   Status: SUCCESS  | {Message}", finalMessage);
            logger.LogInformation("===============================================================");

            return finalMessage;
        }
        catch (Exception ex)
        {
            // Log lỗi cũng cần nổi bật
            logger.LogError("<<< [CRON-END]   Status: FAILED   | Error: {Error}", ex.Message);
            logger.LogInformation("===============================================================");
            throw;
        }
    }
}
