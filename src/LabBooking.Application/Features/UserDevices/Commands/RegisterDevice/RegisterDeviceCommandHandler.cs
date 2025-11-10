using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.UserDevices.Commands.RegisterDevice;

public class RegisterDeviceCommandHandler(
    ILogger<RegisterDeviceCommandHandler> logger,
    IUserDeviceRepository userDeviceRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<RegisterDeviceCommand, Unit>
{
    public async Task<Unit> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            logger.LogWarning("Unable to register device: User is not authenticated.");
            throw new UnauthorizedAccessException("Invalid user.");
        }

        var guidUserId = userId.Value;
        var token = request.PushToken;

        logger.LogInformation("Register device token {Token} for user {UserId}", token, guidUserId);

        var existingDevice = await userDeviceRepository.GetByTokenAsync(token, cancellationToken);

        if (existingDevice != null)
        {
            logger.LogInformation("Token already exists, update UserId {NewUserId}", guidUserId);

            existingDevice.UserId = guidUserId;

            await userDeviceRepository.Update(existingDevice, cancellationToken);
        }
        else
        {
            logger.LogInformation("Token does not exist, create new one.");
            var newDevice = new UserDevice
            {
                UserId = guidUserId,
                PushToken = token
            };
            await userDeviceRepository.AddAsync(newDevice, cancellationToken);
        }

        return Unit.Value;
    }
}
