using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequests;

public class GetDoorRequestsQueryValidator : AbstractValidator<GetDoorRequestsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30, 50];

    // Chỉ cho phép sort theo những cột này để tránh lỗi SQL
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(DoorRequestDto.RequestTime),
        nameof(DoorRequestDto.BookingCode),
        nameof(DoorRequestDto.Status)
    ];

    public GetDoorRequestsQueryValidator()
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
