using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;
using LabBooking.Domain.Exceptions; // Giả sử namespace exception
using MediatR;
using Microsoft.Extensions.Logging;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules;

public class CreateEquipmentMaintainScheduleCommandHandlerTests
{
    // Khai báo các Mock object
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>> _loggerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock;
    private readonly Mock<ILabRoomRepository> _labRoomRepositoryMock;
    private readonly Mock<IEquipmentMaintainScheduleRepository> _scheduleRepositoryMock;

    // Class cần test
    private readonly CreateEquipmentMaintainScheduleCommandHandler _handler;

    public CreateEquipmentMaintainScheduleCommandHandlerTests()
    {
        // Khởi tạo Mock
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>>();
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _labRoomRepositoryMock = new Mock<ILabRoomRepository>();
        _scheduleRepositoryMock = new Mock<IEquipmentMaintainScheduleRepository>();

        // Inject Mock vào Handler
        _handler = new CreateEquipmentMaintainScheduleCommandHandler(
            _mapperMock.Object,
            _loggerMock.Object,
            _mediatorMock.Object,
            _currentUserServiceMock.Object,
            _equipmentRepositoryMock.Object,
            _labRoomRepositoryMock.Object,
            _scheduleRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserNotLoggedIn()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid>(), DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Bạn cần đăng nhập.");
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_When_EquipmentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eqId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Mock Mapper trả về object rỗng để code chạy tiếp
        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>()))
                   .Returns(new EquipmentMaintainSchedule { Details = new List<EquipmentMaintenance>() });

        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync((Equipment)null); // Không tìm thấy

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_UserIsNotManager()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var eqId = Guid.NewGuid();
        var labId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>()))
                   .Returns(new EquipmentMaintainSchedule { Details = new List<EquipmentMaintenance>() });

        // Mock Equipment
        var equipment = new Equipment { Id = eqId, LabRoomId = labId, EquipmentName = "Test EQ" };
        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync(equipment);

        // Mock LabRoom (Manager là người khác)
        var labRoom = new LabRoom { Id = labId, MainManagerId = otherUserId };
        _labRoomRepositoryMock.Setup(x => x.GetByIdAsync(labId, It.IsAny<CancellationToken>())).ReturnsAsync(labRoom);

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbidException>();
    }

    [Fact]
    public async Task Handle_Should_CreateSchedule_And_TriggerJob_When_TimeIsNow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eqId = Guid.NewGuid();
        var labId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Setup Mapper
        var schedule = new EquipmentMaintainSchedule
        {
            StartTime = DateTime.UtcNow, // Giờ hiện tại (Để trigger job)
            EndTime = DateTime.UtcNow.AddHours(1),
            Details = new List<EquipmentMaintenance>()
        };
        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>())).Returns(schedule);

        // Setup Data
        var equipment = new Equipment { Id = eqId, LabRoomId = labId };
        var labRoom = new LabRoom { Id = labId, MainManagerId = userId }; // Manager đúng user

        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync(equipment);
        _labRoomRepositoryMock.Setup(x => x.GetByIdAsync(labId, It.IsAny<CancellationToken>())).ReturnsAsync(labRoom);

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // 1. Phải gọi hàm Save DB
        _scheduleRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Once);

        // 2. Phải gọi Job (Vì StartTime <= Now + 5p)
        _mediatorMock.Verify(x => x.Send(It.IsAny<AutoUpdateEquipmentStatusJobCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CreateSchedule_But_NOT_TriggerJob_When_TimeIsFuture()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eqId = Guid.NewGuid();
        var labId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Setup Mapper
        var schedule = new EquipmentMaintainSchedule
        {
            StartTime = DateTime.UtcNow.AddHours(2), // Tương lai xa (2 tiếng nữa)
            EndTime = DateTime.UtcNow.AddHours(3),
            Details = new List<EquipmentMaintenance>()
        };
        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>())).Returns(schedule);

        var equipment = new Equipment { Id = eqId, LabRoomId = labId };
        var labRoom = new LabRoom { Id = labId, MainManagerId = userId };

        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync(equipment);
        _labRoomRepositoryMock.Setup(x => x.GetByIdAsync(labId, It.IsAny<CancellationToken>())).ReturnsAsync(labRoom);

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now.AddHours(2), DateTime.Now.AddHours(3), "Test");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // 1. Phải gọi hàm Save DB
        _scheduleRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Once);

        // 2. KHÔNG được gọi Job (Vì StartTime > Now + 5p)
        _mediatorMock.Verify(x => x.Send(It.IsAny<AutoUpdateEquipmentStatusJobCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_LabRoomNotFound()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var eqId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        // 1. Mock User đã đăng nhập
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);

        // 2. Mock Mapper (để code không chết ở đoạn map đầu)
        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>()))
                   .Returns(new EquipmentMaintainSchedule { Details = new List<EquipmentMaintenance>() });

        // 3. Mock tìm thấy Thiết bị
        var equipment = new Equipment
        {
            Id = eqId,
            EquipmentName = "Test EQ",
            LabRoomId = labRoomId
        };
        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync(equipment);

        // 4. MOCK QUAN TRỌNG: LabRoom trả về NULL
        // Điều này kích hoạt vế trái: (labRoom == null) -> True
        _labRoomRepositoryMock.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((LabRoom)null);

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Code của bạn throw ForbidException khi labRoom null
        await act.Should().ThrowAsync<ForbidException>()
            .WithMessage($"Không có quyền với thiết bị {equipment.EquipmentName}.");
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_UserIsNotTheManager()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var managerId = Guid.NewGuid(); // ID khác với currentUserId
        var eqId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        // 1. Mock User
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);

        // 2. Mock Mapper
        _mapperMock.Setup(m => m.Map<EquipmentMaintainSchedule>(It.IsAny<object>()))
                   .Returns(new EquipmentMaintainSchedule { Details = new List<EquipmentMaintenance>() });

        // 3. Mock Thiết bị
        var equipment = new Equipment
        {
            Id = eqId,
            EquipmentName = "Test EQ",
            LabRoomId = labRoomId
        };
        _equipmentRepositoryMock.Setup(x => x.GetByIdAsync(eqId)).ReturnsAsync(equipment);

        // 4. MOCK QUAN TRỌNG: LabRoom tồn tại, nhưng MainManagerId là người khác
        // Điều kiện (labRoom == null) -> False.
        // Điều kiện (labRoom.MainManagerId != currentUserId) -> True.
        var labRoom = new LabRoom
        {
            Id = labRoomId,
            MainManagerId = managerId // Khác currentUserId
        };
        _labRoomRepositoryMock.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(labRoom);

        var command = new CreateEquipmentMaintainScheduleCommand(new List<Guid> { eqId }, DateTime.Now, DateTime.Now.AddHours(1), "Test");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbidException>()
            .WithMessage($"Không có quyền với thiết bị {equipment.EquipmentName}.");
    }
}