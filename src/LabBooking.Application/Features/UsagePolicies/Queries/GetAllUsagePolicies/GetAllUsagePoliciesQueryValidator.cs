using LabBooking.Application.Features.UsagePolicies.Dtos;

namespace LabBooking.Application.Features.UsagePolicies.Queries.GetAllUsagePolicies;

public class GetAllUsagePoliciesQueryValidator : AbstractValidator<GetAllUsagePoliciesQuery>
{
    // Giữ nguyên logic PageSize (có thể tùy chỉnh nếu muốn)
    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    // Thay đổi các cột được phép sắp xếp
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(UsagePolicyResponse.Title),
        nameof(UsagePolicyResponse.CreatedDate),
        nameof(UsagePolicyResponse.EffectiveFrom)
    ];

    public GetAllUsagePoliciesQueryValidator()
    {
        // Giữ nguyên logic validation của LabRoom
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        // Cập nhật thông báo lỗi cho SortBy
        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

        // Không cần validate cho bool? IsActive vì null, true, false đều hợp lệ.
    }
}
