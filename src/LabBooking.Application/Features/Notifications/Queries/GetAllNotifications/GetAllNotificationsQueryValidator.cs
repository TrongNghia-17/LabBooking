using LabBooking.Application.Features.Notifications.Dtos;

namespace LabBooking.Application.Features.Notifications.Queries.GetAllNotifications;

public class GetAllNotificationsQueryValidator : AbstractValidator<GetAllNotificationsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30, 50];

    private readonly string[] allowedSortByColumnNames =
        [nameof(NotificationsResponse.Title), nameof(NotificationsResponse.CreatedAt)];

    public GetAllNotificationsQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(", ", allowPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(", ", allowedSortByColumnNames)}]");

    }
}
