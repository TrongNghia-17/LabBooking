namespace LabBooking.Application.Features.Incidents.Commands.Delete;

public class DeleteIncidentValidator : AbstractValidator<DeleteIncidentCommand>
{
    public DeleteIncidentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID sự cố không được để trống.");
    }
}