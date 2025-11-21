using FluentValidation;
using LabBooking.Domain.Entities;

namespace LabBooking.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        // --- Validate Chung ---
        RuleFor(x => x.LabRoomId).NotEmpty().WithMessage("Lab Room ID is required.");
        RuleFor(x => x.CreatedById).NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NumberOfParticipants).GreaterThan(0);
        RuleFor(x => x.Slots).NotEmpty().WithMessage("Must select at least one slot.");
        RuleFor(x => x.Type).IsInEnum();

        // --- Validate theo Type: Teaching ---
        When(x => x.Type == BookingType.Teaching, () => {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course ID is required for Teaching bookings.");
        });

        // --- Validate theo Type: Project ---
        When(x => x.Type == BookingType.Project, () => {
            RuleFor(x => x.Project)
                .NotNull()
                .WithMessage("Project information is required.");

            RuleFor(x => x.Project!.ProjectName)
                .NotEmpty()
                .When(x => x.Project != null)
                .WithMessage("Project Name is required.");
        });

        // --- Validate theo Type: UniversityEvent (Priority) ---
        // Giả sử UniversityEvent map với Priority trong enum BookingType (bạn cần check lại enum này trong code của bạn)
        // Nếu trong Enum BookingType của bạn chưa có UniversityEvent, hãy thêm vào hoặc map tương ứng.
        // Ở đây tôi giả sử BookingType chỉ có Teaching(0) và Project(1). 
        // Nếu bạn dùng logic Priority riêng, hãy điều chỉnh điều kiện When.

        // Ví dụ: Nếu bạn định nghĩa Priority là một Type riêng trong Enum BookingType
        
        When(x => x.Type == BookingType.UniversityEvent, () => {
             RuleFor(x => x.PriorityDetail)
                .NotNull()
                .WithMessage("Priority justification is required.");
                
             RuleFor(x => x.PriorityDetail!.Justification)
                .NotEmpty()
                .When(x => x.PriorityDetail != null);
        });

        When(x => x.ExternalEquipments != null && x.ExternalEquipments.Any(), () => {
            RuleForEach(x => x.ExternalEquipments).ChildRules(items => {
                items.RuleFor(e => e.Name).NotEmpty().WithMessage("Tên thiết bị mang vào không được để trống.");
                items.RuleFor(e => e.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
            });
        });

    }
}