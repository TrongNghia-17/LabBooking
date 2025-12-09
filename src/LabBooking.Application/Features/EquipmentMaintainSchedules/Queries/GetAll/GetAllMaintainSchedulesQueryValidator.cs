namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

public class GetAllMaintainSchedulesQueryValidator : AbstractValidator<GetAllMaintainSchedulesQuery>
{
    private readonly int[] allowPageSizes = [5, 10, 15, 30, 50];

    public GetAllMaintainSchedulesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .Must(x => allowPageSizes.Contains(x))
            .WithMessage($"PageSize phải thuộc: {string.Join(",", allowPageSizes)}");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
    }
}