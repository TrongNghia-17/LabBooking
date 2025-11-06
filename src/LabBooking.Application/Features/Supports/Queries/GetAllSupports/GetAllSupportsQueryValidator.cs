using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetAllSupports;

public class GetAllSupportsQueryValidator : AbstractValidator<GetAllSupportsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    private readonly string[] allowedSortByColumnNames =
        [nameof(SupportsResponse.Title), nameof(SupportsResponse.Content)];

    public GetAllSupportsQueryValidator()
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
