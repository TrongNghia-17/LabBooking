using LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

namespace LabBooking.Application.Features.Authentication.Queries.GetProfile;

public class AutoUpdateEquipmentStatusJobHandler(
    IEquipmentMaintainScheduleRepository repository,
    ILogger<AutoUpdateEquipmentStatusJobHandler> logger
    ) : IRequestHandler<AutoUpdateEquipmentStatusJobCommand, string>
{
    public async Task<string> Handle(AutoUpdateEquipmentStatusJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Chỉ chạy logic bảo trì, xong!
            var resultMessage = await repository.ProcessAutomatedMaintenanceAsync(cancellationToken);

            logger.LogInformation("Maintenance job completed: {Result}", resultMessage);

            return resultMessage;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi chạy Job cập nhật bảo trì");
            throw;
        }
    }
}
