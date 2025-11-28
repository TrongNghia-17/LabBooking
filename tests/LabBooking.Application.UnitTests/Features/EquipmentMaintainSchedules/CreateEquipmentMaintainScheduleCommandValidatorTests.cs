using FluentValidation.TestHelper;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules;

public class CreateEquipmentMaintainScheduleCommandValidatorTests
{
    private readonly CreateEquipmentMaintainScheduleCommandValidator _validator;

    public CreateEquipmentMaintainScheduleCommandValidatorTests()
    {
        _validator = new CreateEquipmentMaintainScheduleCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartTime_Is_In_Past()
    {
        // ARRANGE
        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: Guid.NewGuid(),
            StartTime: DateTime.UtcNow.AddHours(-1),
            EndTime: DateTime.UtcNow.AddHours(1),
            Description: "Test"
        );

        // ACT
        var result = _validator.TestValidate(command);

        // ASSERT
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
              .WithErrorMessage("Thời gian bắt đầu bảo trì không được ở quá khứ.");
    }

    [Fact]
    public void Should_Have_Error_When_EndTime_Before_StartTime()
    {
        // ARRANGE
        var now = DateTime.UtcNow.AddHours(1);
        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: Guid.NewGuid(),
            StartTime: now,
            EndTime: now.AddHours(-1),
            Description: "Test"
        );

        // ACT
        var result = _validator.TestValidate(command);

        // ASSERT
        result.ShouldHaveValidationErrorFor(x => x.EndTime)
              .WithErrorMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Dates_Are_Valid()
    {
        // ARRANGE
        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: Guid.NewGuid(),
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(3),
            Description: "Mô tả hợp lệ"
        );

        // ACT
        var result = _validator.TestValidate(command);

        // ASSERT
        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
        result.ShouldNotHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void Should_Have_Error_When_EquipmentId_Is_Empty()
    {
        var command = new CreateEquipmentMaintainScheduleCommand(
            Guid.Empty,
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            "Test"
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EquipmentId)
              .WithErrorMessage("Vui lòng chọn thiết bị.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new CreateEquipmentMaintainScheduleCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            ""
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Vui lòng nhập mô tả bảo trì.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Long()
    {
        var command = new CreateEquipmentMaintainScheduleCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            new string('a', 1001)
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Mô tả không được vượt quá 1000 ký tự.");
    }

    [Fact]
    public void Should_Pass_When_StartTime_Is_Slightly_In_Past_Within_Tolerance()
    {
        var startTime = DateTime.UtcNow.AddMinutes(-3);

        var command = new CreateEquipmentMaintainScheduleCommand(
            Guid.NewGuid(),
            startTime,
            startTime.AddHours(1),
            "Test"
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
    }
}
