namespace LabBooking.Application.Features.UserDevices.Commands.RegisterDevice;

public record RegisterDeviceCommand(string PushToken) : IRequest<Unit>;

