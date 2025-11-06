using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAllLabRooms;

public class GetAllLabRoomsQueryValidator : AbstractValidator<GetAllLabRoomsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    private readonly string[] allowedSortByColumnNames =
    [
        nameof(LabRoomResponse.LabName),
        nameof(LabRoomResponse.Location),
        nameof(LabRoomResponse.CreatedDate)
    ];

    public GetAllLabRoomsQueryValidator()
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
