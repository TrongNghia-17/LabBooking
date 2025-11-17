using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAllEquipmentMaintainSchedules;

public class GetAllEquipmentMaintainSchedulesQueryValidator : AbstractValidator<GetAllEquipmentMaintainSchedulesQuery>
{
    // Giống LabRoom
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    // Cập nhật các cột được phép sort
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(EquipmentMaintainScheduleResponse.StartTime),
        nameof(EquipmentMaintainScheduleResponse.EndTime),
        nameof(EquipmentMaintainScheduleResponse.EquimentpMaintainStatus) // Tên DTO
    ];

    public GetAllEquipmentMaintainSchedulesQueryValidator()
    {
        // Validation phân trang (giống LabRoom)
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        // Validation sắp xếp (giống LabRoom)
        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

        // Validation cho Status (MỚI)
        RuleFor(r => r.Status)
            .IsInEnum()
            .When(r => r.Status.HasValue)
            .WithMessage("Invalid status value.");
    }
}
