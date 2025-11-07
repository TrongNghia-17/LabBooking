using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;

public class GetAllEquipmentsQueryValidator : AbstractValidator<GetAllEquipmentsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    // Cập nhật các cột được phép sắp xếp
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(EquipmentResponse.EquipmentName),
        nameof(EquipmentResponse.Status),
        nameof(EquipmentResponse.IsAvailable),
        nameof(EquipmentResponse.LabRoomId)
    ];

    public GetAllEquipmentsQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}