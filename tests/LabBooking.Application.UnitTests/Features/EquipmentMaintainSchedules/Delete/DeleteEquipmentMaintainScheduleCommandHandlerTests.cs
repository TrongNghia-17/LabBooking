using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules.Delete;

public class DeleteEquipmentMaintainScheduleCommandHandlerTests
{
    private readonly Mock<IEquipmentMaintainScheduleRepository> _mockScheduleRepo;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepo;
    private readonly Mock<ILabRoomRepository> _mockLabRoomRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<DeleteEquipmentMaintainScheduleCommandHandler>> _mockLogger;

    private readonly DeleteEquipmentMaintainScheduleCommandHandler _handler;

    public DeleteEquipmentMaintainScheduleCommandHandlerTests()
    {
        _mockScheduleRepo = new Mock<IEquipmentMaintainScheduleRepository>();
        _mockEquipmentRepo = new Mock<IEquipmentRepository>();
        _mockLabRoomRepo = new Mock<ILabRoomRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<DeleteEquipmentMaintainScheduleCommandHandler>>();

        _handler = new DeleteEquipmentMaintainScheduleCommandHandler(
            _mockLogger.Object,
            _mockScheduleRepo.Object,
            _mockEquipmentRepo.Object,
            _mockLabRoomRepo.Object,
            _mockCurrentUserService.Object
        );
    }

    [Fact]
    public async Task Handle_Should_DeleteSchedule_And_RollbackStatus_When_EquipmentIsMaintain()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        // 1. Mock Login
        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        // 2. Mock Schedule (Status NotYet -> Được phép xóa)
        var schedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquipmentId = equipmentId,
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        // 3. Mock Equipment (Status Maintain -> Cần Rollback)
        var equipment = new Equipment
        {
            Id = equipmentId,
            LabRoomId = labRoomId,
            Status = EquipmentStatus.Maintain, // Đang bảo trì
            IsAvailable = false
        };
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId)).ReturnsAsync(equipment);

        // 4. Mock LabRoom (User là Manager)
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        var command = new DeleteEquipmentMaintainScheduleCommand(scheduleId);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        // 1. Phải gọi Update Equipment để trả về Available
        _mockEquipmentRepo.Verify(x => x.Update(It.Is<Equipment>(e =>
            e.Status == EquipmentStatus.Available &&
            e.IsAvailable == true
        )), Times.Once);

        // 2. Phải gọi Delete Schedule
        _mockScheduleRepo.Verify(x => x.DeleteAsync(schedule, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_DeleteSchedule_But_KeepStatus_When_EquipmentIsNotMaintain()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        var schedule = new EquipmentMaintainSchedule { Id = scheduleId, EquipmentId = equipmentId, EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>())).ReturnsAsync(schedule);

        // Thiết bị đang Broken (Hỏng) -> Xóa lịch tương lai không được tự ý sửa thành Available
        var equipment = new Equipment
        {
            Id = equipmentId,
            LabRoomId = labRoomId,
            Status = EquipmentStatus.Broken,
            IsAvailable = false
        };
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId)).ReturnsAsync(equipment);

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        // ACT
        await _handler.Handle(new DeleteEquipmentMaintainScheduleCommand(scheduleId), CancellationToken.None);

        // ASSERT
        // 1. KHÔNG được gọi Update Equipment
        _mockEquipmentRepo.Verify(x => x.Update(It.IsAny<Equipment>()), Times.Never);

        // 2. Vẫn gọi Delete Schedule
        _mockScheduleRepo.Verify(x => x.DeleteAsync(schedule, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowBadRequest_When_ScheduleIsDone()
    {
        // ARRANGE
        var scheduleId = Guid.NewGuid();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        // Lịch đã Done -> Không được xóa
        var schedule = new EquipmentMaintainSchedule
        {
            Id = scheduleId,
            EquimentpMaintainStatus = EquimentpMaintainStatus.Done
        };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(new DeleteEquipmentMaintainScheduleCommand(scheduleId), CancellationToken.None));

        exception.Message.Should().Contain("đã hoàn thành");

        // Đảm bảo không gọi Delete
        _mockScheduleRepo.Verify(x => x.DeleteAsync(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
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

        var schedule = new EquipmentMaintainSchedule { Id = scheduleId, EquipmentId = equipmentId, EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet };
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>())).ReturnsAsync(schedule);

        var equipment = new Equipment { Id = equipmentId, LabRoomId = labRoomId };
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId)).ReturnsAsync(equipment);

        // Phòng Lab do người khác quản lý
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = otherManagerId });

        // ACT & ASSERT
        await Assert.ThrowsAsync<ForbidException>(() =>
            _handler.Handle(new DeleteEquipmentMaintainScheduleCommand(scheduleId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_When_ScheduleDoesNotExist()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EquipmentMaintainSchedule?)null);

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteEquipmentMaintainScheduleCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserNotLoggedIn()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns((Guid?)null);

        // ACT & ASSERT
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new DeleteEquipmentMaintainScheduleCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_When_LabRoomDoesNotExist()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EquipmentMaintainSchedule
            {
                Id = scheduleId,
                EquipmentId = equipmentId,
                EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
            });

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        // LabRoom trả về NULL
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LabRoom?)null);

        var command = new DeleteEquipmentMaintainScheduleCommand(scheduleId);

        // ACT & ASSERT
        // Sửa: Mong đợi NotFoundException
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_User_IsNot_Manager()
    {
        // ARRANGE
        var currentUserId = Guid.NewGuid();      // User đang đăng nhập
        var otherManagerId = Guid.NewGuid();     // Manager thật của phòng (người khác)

        var scheduleId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        // 1. Setup User
        _mockCurrentUserService.Setup(x => x.UserId).Returns(currentUserId);

        // 2. Setup Schedule
        _mockScheduleRepo.Setup(x => x.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EquipmentMaintainSchedule
            {
                Id = scheduleId,
                EquipmentId = equipmentId,
                EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
            });

        // 3. Setup Equipment
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        // 4. SETUP QUAN TRỌNG: LabRoom tồn tại nhưng MainManagerId KHÁC currentUserId
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom
            {
                Id = labRoomId,
                MainManagerId = otherManagerId // <--- Khác currentUserId
            });

        var command = new DeleteEquipmentMaintainScheduleCommand(scheduleId);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ForbidException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Bạn không có quyền xóa lịch bảo trì của thiết bị này.");
    }
}
