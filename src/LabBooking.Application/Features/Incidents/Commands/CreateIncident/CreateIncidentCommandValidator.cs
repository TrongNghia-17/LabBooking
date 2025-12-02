namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator(IEquipmentRepository equipmentRepository)
    {
        RuleFor(x => x.LabRoomId)
            .NotEmpty().WithMessage("Vui lòng chọn phòng Lab xảy ra sự cố.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại sự cố không hợp lệ.");


        RuleFor(x => x.ImportanceLevel)
            .IsInEnum().WithMessage("Mức độ quan trọng không hợp lệ.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Vui lòng nhập mô tả sự cố.")
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.EquipmentId)
            .NotEmpty()
            .When(x => x.Type == IncidentType.EquipmentFailure)
            .WithMessage("Vui lòng chọn thiết bị bị hỏng.");

        RuleFor(x => x.EquipmentId)
            .Null()
            .When(x => x.Type != IncidentType.EquipmentFailure)
            .WithMessage("Không được chọn thiết bị nếu loại sự cố không phải là 'Hư hỏng thiết bị'.");

        RuleFor(x => x.EquipmentId)
            .MustAsync(async (command, equipmentId, token) =>
            {
                // Nếu không chọn thiết bị thì bỏ qua check này (để rule NotEmpty bên trên lo)
                if (!equipmentId.HasValue) return true;

                // Gọi Repo kiểm tra
                return await equipmentRepository.IsEquipmentInLabAsync(equipmentId.Value, command.LabRoomId, token);
            })
            .When(x => x.Type == IncidentType.EquipmentFailure && x.EquipmentId.HasValue)
            .WithMessage("Thiết bị bạn chọn không thuộc phòng Lab này. Vui lòng kiểm tra lại.");
    }
}
