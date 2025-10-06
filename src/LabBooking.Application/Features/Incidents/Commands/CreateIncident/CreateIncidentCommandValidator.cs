namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(c => c.LabRoomId)
            .NotEmpty()
            .WithMessage("Lab Room ID cannot be empty.");

        RuleFor(c => c.ReportedById)
            .NotEmpty()
            .WithMessage("Reported By ID cannot be empty.");

        var validNames = string.Join(", ", Enum.GetNames(typeof(IncidentType)));
        RuleFor(c => c.Type)
            .NotEmpty().WithMessage("Incident type is required.")
            .Must(v => Enum.TryParse<IncidentType>(v, true, out _))
            .WithMessage($"Incident type is invalid. Valid values are: {validNames}. You may send either the name (e.g., \"Fire\") or the numeric value (e.g., 0).");

        RuleFor(c => c.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description cannot be longer than 500 characters.");
    }
}
