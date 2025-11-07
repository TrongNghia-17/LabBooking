using LabBooking.Application.Features.Users.Dtos;

namespace LabBooking.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    // Các PageSize hợp lệ
    private readonly int[] allowPageSizes = [5, 10, 15, 30, 50];

    // Các cột được phép sắp xếp (lấy từ tên property của UserResponse)
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(UserResponse.UserName),
        nameof(UserResponse.Email),
        nameof(UserResponse.Major),
        nameof(UserResponse.RegistrationDate)
    ];

    public GetAllUsersQueryValidator()
    {
        // Rule cho PageNumber: Phải >= 1
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        // Rule cho PageSize: Phải nằm trong danh sách allowPageSizes
        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(", ", allowPageSizes)}]");

        // Rule cho SortBy: Phải là null hoặc nằm trong danh sách allowedSortByColumnNames
        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(", ", allowedSortByColumnNames)}]");
    }
}
