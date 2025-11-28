namespace LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

public class AutoUpdateEquipmentStatusJobHandler(
    IEquipmentMaintainScheduleRepository repository,
    ILogger<AutoUpdateEquipmentStatusJobHandler> logger
    ) : IRequestHandler<AutoUpdateEquipmentStatusJobCommand, string>
{
    public async Task<string> Handle(AutoUpdateEquipmentStatusJobCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("CronJob: Bắt đầu quét trạng thái thiết bị...");

        var resultMessage = await repository.ProcessAutoStatusUpdatesAsync(cancellationToken);

        logger.LogInformation(message: resultMessage);

        return resultMessage;
    }
}
