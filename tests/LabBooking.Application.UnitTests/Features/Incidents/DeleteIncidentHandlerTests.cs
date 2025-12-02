using LabBooking.Application.Features.Incidents.Commands.Delete;
using LabBooking.Application.Services.Users;
using LabBooking.Domain.Enums;
using LabBooking.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LabBooking.Application.UnitTests.Features.Incidents;

public class DeleteIncidentHandlerTests
{
    // 1. Khai báo Mock
    private readonly Mock<IIncidentRepository> _incidentRepoMock;
    private readonly Mock<IEquipmentRepository> _equipmentRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ILogger<DeleteIncidentHandler>> _loggerMock;

    // Handler cần test
    private readonly DeleteIncidentHandler _handler;

    public DeleteIncidentHandlerTests()
    {
        _incidentRepoMock = new Mock<IIncidentRepository>();
        _equipmentRepoMock = new Mock<IEquipmentRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<DeleteIncidentHandler>>();

        _handler = new DeleteIncidentHandler(
            _incidentRepoMock.Object,
            _equipmentRepoMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object
        );
    }

    // =========================================================================
    // NHÓM 1: VALIDATION & AUTH (Đăng nhập, Tồn tại)
    // =========================================================================

    [Fact]
    public async Task Handle_ShouldThrow_Unauthorized_WhenUserNotLoggedIn()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
        var command = new DeleteIncidentCommand(Guid.NewGuid());

        // Act (QUAN TRỌNG: Bọc vào Func)
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Bạn cần đăng nhập.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_NotFound_WhenIncidentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Repo trả về null
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Incident?)null);

        var command = new DeleteIncidentCommand(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // =========================================================================
    // NHÓM 2: AUTHORIZATION (Quyền hạn)
    // =========================================================================

    [Fact]
    public async Task Handle_ShouldThrow_Forbid_WhenUserHasNoPermission()
    {
        // Arrange: User thường, xóa bài người khác, không phải Manager
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = otherUserId,
            LabRoom = new LabRoom { MainManagerId = otherUserId }
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        await act.Should().ThrowAsync<ForbidException>();
    }

    // =========================================================================
    // NHÓM 3: BUSINESS RULES (Logic Nghiệp vụ)
    // =========================================================================

    [Fact]
    public async Task Handle_ShouldThrow_BadRequest_WhenEquipmentIsMaintaining()
    {
        // Arrange: Máy đang bảo trì (Maintain) -> Cấm xóa báo cáo
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId,
            IsResolved = false,
            // Logic MỚI của bạn: Check Equipment Status = Maintain
            Equipment = new Equipment { Status = EquipmentStatus.Maintain }
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*đang được bảo trì*"); // Check message chứa từ khóa
    }

    [Fact]
    public async Task Handle_ShouldThrow_BadRequest_WhenIncidentIsResolved()
    {
        // Arrange: Đã xử lý xong -> Cấm xóa
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId,
            IsResolved = true // Đã xong
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*đã được xử lý xong*");
    }

    [Fact]
    public async Task Handle_Should_RevertStatusToAvailable_WhenEquipmentWasBroken()
    {
        // Arrange: Hủy báo cáo hỏng -> Máy phải quay về Available
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Status = EquipmentStatus.Broken, // Đang hỏng
            IsAvailable = false
        };

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId,
            IsResolved = false,
            Type = IncidentType.EquipmentFailure,
            Equipment = equipment
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        // Với trường hợp thành công, ta gọi await trực tiếp để lấy kết quả
        var result = await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        result.Should().BeTrue();

        // 1. Kiểm tra trạng thái máy đã đổi chưa
        equipment.Status.Should().Be(EquipmentStatus.Available);
        equipment.IsAvailable.Should().BeTrue();

        // 2. Kiểm tra hàm Update có được gọi không
        _equipmentRepoMock.Verify(x => x.UpdateAsync(equipment, It.IsAny<CancellationToken>()), Times.Once);

        // 3. Kiểm tra hàm Delete có được gọi không
        _incidentRepoMock.Verify(x => x.DeleteAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotRevertStatus_When_EquipmentIsNotBroken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Status = EquipmentStatus.Available, // <--- KHÁC Broken
            IsAvailable = true
        };

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId,
            IsResolved = false,
            Type = IncidentType.EquipmentFailure, // Đúng loại
            Equipment = equipment // Có thiết bị
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        // 1. KHÔNG ĐƯỢC gọi Update (Vì status không phải Broken nên không cần revert)
        _equipmentRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Never);

        // 2. Vẫn xóa Incident bình thường
        _incidentRepoMock.Verify(x => x.DeleteAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotRevertStatus_When_EquipmentIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId,
            IsResolved = false,
            Type = IncidentType.EquipmentFailure,
            Equipment = null // <--- NULL (Dù là báo hỏng)
        };
        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        // KHÔNG gọi Update thiết bị
        _equipmentRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Never);
        _incidentRepoMock.Verify(x => x.DeleteAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_Forbid_When_LabRoomIsNull_And_UserIsNotAdminOrReporter()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var reporterId = Guid.NewGuid(); // Người khác báo

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>()); // Không phải Admin

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = reporterId, // Không phải Reporter
            LabRoom = null // <--- NULL (Cover dấu ?.)
        };

        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        // LabRoom null -> isManager = false -> User không có quyền gì cả -> Throw Forbid
        await act.Should().ThrowAsync<ForbidException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_Forbid_When_LabRoomIsNull_And_UserHasNoOtherRights()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>()); // Không phải Admin

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = otherId, // Không phải Reporter
            LabRoom = null // <--- QUAN TRỌNG: LabRoom = NULL
                           // Khi LabRoom null -> isManager = false -> Code chạy vào nhánh false của dấu ?.
        };

        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        await act.Should().ThrowAsync<ForbidException>();
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenUserIsReporter_But_LabRoomExistsAndNotManager()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var managerId = Guid.NewGuid(); // Người khác làm Manager

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _currentUserServiceMock.Setup(x => x.Roles).Returns(new List<string>());

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ReportedById = userId, // Là Reporter (Hợp lệ)
            IsResolved = false,
            // QUAN TRỌNG: LabRoom tồn tại, nhưng Manager là người khác
            // Điều này ép code chạy qua nhánh: LabRoom != null -> MainManagerId != userId -> isManager = false
            LabRoom = new LabRoom { MainManagerId = managerId }
        };

        _incidentRepoMock.Setup(x => x.GetByIdWithDetailsAsync(incident.Id, default)).ReturnsAsync(incident);

        // Act
        var result = await _handler.Handle(new DeleteIncidentCommand(incident.Id), default);

        // Assert
        result.Should().BeTrue();
        _incidentRepoMock.Verify(x => x.DeleteAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
    }
}