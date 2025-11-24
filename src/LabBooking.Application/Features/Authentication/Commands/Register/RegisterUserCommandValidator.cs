using LabBooking.Application.Resources;
using Microsoft.Extensions.Localization;

namespace LabBooking.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Validator for the <see cref="RegisterUserCommand"/>.
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(
        UserManager<User> userManager,
        IStringLocalizer<AuthMessages> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalid"])
            .MustAsync(async (email, token) =>
                await userManager.FindByEmailAsync(email) == null)
            .WithMessage(localizer["EmailInUse"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"])
            .MinimumLength(6).WithMessage(localizer["PasswordTooShort"]);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage(localizer["ConfirmPasswordMismatch"]);
    }
}
