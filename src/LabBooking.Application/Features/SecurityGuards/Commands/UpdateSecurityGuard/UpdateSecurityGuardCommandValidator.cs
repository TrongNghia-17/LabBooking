namespace LabBooking.Application.Features.SecurityGuards.Commands.UpdateSecurityGuard
{
    public class UpdateSecurityGuardCommandValidator : AbstractValidator<UpdateSecurityGuardCommand>
    {
        public UpdateSecurityGuardCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10,11}$").WithMessage("Phone number must be valid.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}
