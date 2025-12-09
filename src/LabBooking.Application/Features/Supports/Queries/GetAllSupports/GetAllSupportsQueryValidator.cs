using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetAllSupports;

/// <summary>
/// Defines the validation rules for the <see cref="GetAllSupportsQuery"/>.
/// </summary>
public class GetAllSupportsQueryValidator : AbstractValidator<GetAllSupportsQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    private readonly string[] allowedSortByColumnNames =
        [nameof(SupportsResponse.Title), nameof(SupportsResponse.Content)];

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllSupportsQueryValidator"/> class
    /// and configures the validation rules.
    /// </summary>
    public GetAllSupportsQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

        RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Trạng thái hỗ trợ không hợp lệ.");
    }
}
