using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules.Update;

public class UpdateEquipmentMaintainScheduleCommandHandlerTests
{
    private readonly Mock<IEquipmentMaintainScheduleRepository> _mockScheduleRepo;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepo;
    private readonly Mock<ILabRoomRepository> _mockLabRoomRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UpdateEquipmentMaintainScheduleCommandHandler>> _mockLogger;

    private readonly UpdateEquipmentMaintainScheduleCommandHandler _handler;

    public UpdateEquipmentMaintainScheduleCommandHandlerTests()
    {
        _mockScheduleRepo = new Mock<IEquipmentMaintainScheduleRepository>();
        _mockEquipmentRepo = new Mock<IEquipmentRepository>();
        _mockLabRoomRepo = new Mock<ILabRoomRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UpdateEquipmentMaintainScheduleCommandHandler>>();

        _handler = new UpdateEquipmentMaintainScheduleCommandHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockScheduleRepo.Object,
            _mockEquipmentRepo.Object,
            _mockLabRoomRepo.Object,
            _mockCurrentUserService.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Update_When_InputIsValid_And_NoOverlap()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            Id = scheduleId,
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(4),
            Description = "Updated Description"
        };

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        var existingSchedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(3),
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSchedule);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        _mockScheduleRepo.Setup(x => x.IsOverlapAsync(equipmentId, command.StartTime, command.EndTime, scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        _mockScheduleRepo.Verify(x => x.Update(existingSchedule, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map(command, existingSchedule), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowBadRequest_When_OverlapDetected()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();

        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            Id = scheduleId,
            StartTime = DateTime.UtcNow.AddHours(5),
            EndTime = DateTime.UtcNow.AddHours(6)
        };

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        var existingSchedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };

        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSchedule);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = Guid.NewGuid() });
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new LabRoom { MainManagerId = userId });

        _mockScheduleRepo.Setup(x => x.IsOverlapAsync(equipmentId, command.StartTime, command.EndTime, scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // ACT & ASSERT
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));

        _mockScheduleRepo.Verify(x => x.Update(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowBadRequest_When_StatusIsDone()
    {
        // ARRANGE
        var scheduleId = Guid.NewGuid();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var existingSchedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquimentpMaintainStatus = EquimentpMaintainStatus.Done // Đã xong
        };

        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSchedule);

        var command = new UpdateEquipmentMaintainScheduleCommand { Id = scheduleId };

        // ACT & ASSERT
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        ex.Message.Should().Contain("đã hoàn thành");
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_UserIsNotManager()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var otherManagerId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        var existingSchedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>())).ReturnsAsync(existingSchedule);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId)).ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = otherManagerId });

        var command = new UpdateEquipmentMaintainScheduleCommand { Id = scheduleId };

        // ACT & ASSERT
        await Assert.ThrowsAsync<ForbidException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_NotCheckOverlap_When_TimeIsNotChanged()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            Id = scheduleId,
            StartTime = now,
            EndTime = now.AddHours(1),
            Description = "Chỉ sửa mô tả"
        };

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        var existingSchedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            StartTime = now,
            EndTime = now.AddHours(1),
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSchedule);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        _mockScheduleRepo.Verify(x => x.IsOverlapAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockScheduleRepo.Verify(x => x.Update(existingSchedule, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserNotLoggedIn()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns((Guid?)null);

        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Bạn cần đăng nhập.");
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_User_IsNot_Manager_Of_LabRoom()
    {
        // ARRANGE
        var currentUserId = Guid.NewGuid();
        var differentManagerId = Guid.NewGuid();

        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(currentUserId);

        var schedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet // Trạng thái hợp lệ
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom
            {
                Id = labRoomId,
                MainManagerId = differentManagerId
            });

        var command = new UpdateEquipmentMaintainScheduleCommand
        {
            Id = scheduleId,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ForbidException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Bạn không có quyền chỉnh sửa lịch bảo trì của phòng Lab này.");
    }
}
