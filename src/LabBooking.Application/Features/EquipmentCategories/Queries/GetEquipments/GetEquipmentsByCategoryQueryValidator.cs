namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

public class GetEquipmentsByCategoryQueryValidator : AbstractValidator<GetEquipmentsByCategoryQuery>
{
    public GetEquipmentsByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID không được để trống.")
            .NotNull();
    }
}
