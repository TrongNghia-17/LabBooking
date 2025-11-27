using FluentValidation.TestHelper;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules;

public class CreateEquipmentMaintainScheduleCommandValidatorTests
{
    private readonly CreateEquipmentMaintainScheduleCommandValidator _validator;

    public CreateEquipmentMaintainScheduleCommandValidatorTests()
    {
        // Lưu ý: Validator của bạn không có Dependencies phức tạp nên khởi tạo trực tiếp
        _validator = new CreateEquipmentMaintainScheduleCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartTime_Is_In_Past()
    {
        // ARRANGE
        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: Guid.NewGuid(),
            StartTime: DateTime.UtcNow.AddHours(-1), // Quá khứ
            EndTime: DateTime.UtcNow.AddHours(1),
            Description: "Test"
        );

        // ACT
        var result = _validator.TestValidate(command);

        // ASSERT
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
              .WithErrorMessage("Thời gian bắt đầu bảo trì phải lớn hơn thời gian hiện tại.");
    }

    [Fact]
    public void Should_Have_Error_When_EndTime_Before_StartTime()
    {
        // ARRANGE
        var now = DateTime.UtcNow.AddHours(1);
        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: Guid.NewGuid(),
            StartTime: now,
            EndTime: now.AddHours(-1), // Kết thúc trước khi bắt đầu
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
            StartTime: DateTime.UtcNow.AddHours(1), // Tương lai
            EndTime: DateTime.UtcNow.AddHours(3),   // Sau StartTime
            Description: "Mô tả hợp lệ"
        );

        // ACT
        var result = _validator.TestValidate(command);

        // ASSERT
        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
        result.ShouldNotHaveValidationErrorFor(x => x.EndTime);
    }
}
