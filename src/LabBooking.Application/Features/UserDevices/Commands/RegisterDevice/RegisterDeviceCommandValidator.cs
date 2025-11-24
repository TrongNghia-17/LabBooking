namespace LabBooking.Application.Features.UserDevices.Commands.RegisterDevice;

public class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.PushToken)
            .NotEmpty()
            .WithMessage("PushToken cannot be empty.");
    }
}
