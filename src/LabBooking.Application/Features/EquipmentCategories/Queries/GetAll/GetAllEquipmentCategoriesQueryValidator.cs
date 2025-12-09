namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public class GetAllEquipmentCategoriesQueryValidator : AbstractValidator<GetAllEquipmentCategoriesQuery>
{
    private int[] allowedPageSizes = [5, 10, 15, 30, 50];
    private string[] allowedSortBy = ["name", "description", "equipmentCount"];

    public GetAllEquipmentCategoriesQueryValidator()
    {
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(x => allowedPageSizes.Contains(x))
            .WithMessage($"PageSize phải thuộc: [{string.Join(",", allowedPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(x => allowedSortBy.Contains(x.ToLower()))
            .When(r => !string.IsNullOrEmpty(r.SortBy))
            .WithMessage($"SortBy chỉ chấp nhận: {string.Join(", ", allowedSortBy)}");
    }
}
