using FluentValidation.TestHelper;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules.Update;

public class UpdateEquipmentMaintainScheduleCommandValidatorTests
{
    private readonly UpdateEquipmentMaintainScheduleCommandValidator _validator;

    public UpdateEquipmentMaintainScheduleCommandValidatorTests()
    {
        _validator = new UpdateEquipmentMaintainScheduleCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartTime_Is_Empty()
    {
        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            StartTime = default,
            EndTime = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Should_Have_Error_When_EndTime_Before_StartTime()
    {
        var now = DateTime.UtcNow;
        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            StartTime = now,
            EndTime = now.AddHours(-1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime)
              .WithErrorMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");
    }

    [Fact]
    public void Should_Pass_When_Dates_Are_Valid()
    {
        var now = DateTime.UtcNow;
        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            StartTime = now,
            EndTime = now.AddHours(2),
            Description = "Valid Update"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
        result.ShouldNotHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Description = new string('a', 1001)
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
