using LabBooking.Application.Features.DoorRequests.Commands.Create;
using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;
using LabBooking.Application.Services.Users;
using LabBooking.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LabBooking.Application.UnitTests.Features.DoorRequests;

public class CreateDoorRequestHandlerTests
{
    private readonly Mock<IDoorRequestRepository> _doorRepoMock;
    private readonly Mock<ILabRoomRepository> _labRepoMock;
    private readonly Mock<ICurrentUserService> _userMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateDoorRequestHandler>> _loggerMock;
    private readonly CreateDoorRequestHandler _handler;

    public CreateDoorRequestHandlerTests()
    {
        _doorRepoMock = new Mock<IDoorRequestRepository>();
        _labRepoMock = new Mock<ILabRoomRepository>();
        _userMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateDoorRequestHandler>>();

        _handler = new CreateDoorRequestHandler(
            _doorRepoMock.Object,
            _labRepoMock.Object,
            _userMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // 1. Test trường hợp chưa đăng nhập
    [Fact]
    public async Task Handle_ShouldThrow_Unauthorized_When_UserNotLoggedIn()
    {
        // Arrange
        _userMock.Setup(x => x.UserId).Returns((Guid?)null); // Giả lập User null
        var command = new CreateDoorRequestCommand(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // 2. Test trường hợp phòng không tồn tại
    [Fact]
    public async Task Handle_ShouldThrow_NotFound_When_LabRoomDoesNotExist()
    {
        // Arrange
        _userMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        var command = new CreateDoorRequestCommand(Guid.NewGuid());

        // Giả lập Repo trả về null
        _labRepoMock.Setup(x => x.GetByIdAsync(command.LabRoomId, default))
            .ReturnsAsync((LabRoom?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // 3. Test trường hợp Spam (Đang có request pending)
    [Fact]
    public async Task Handle_ShouldThrow_BadRequest_When_HasPendingRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userMock.Setup(x => x.UserId).Returns(userId);
        var command = new CreateDoorRequestCommand(Guid.NewGuid());

        // Giả lập phòng tồn tại
        _labRepoMock.Setup(x => x.GetByIdAsync(command.LabRoomId, default))
            .ReturnsAsync(new LabRoom { Id = command.LabRoomId });

        // Giả lập Repo báo "Có pending" (True)
        _doorRepoMock.Setup(x => x.HasPendingRequestAsync(userId, command.LabRoomId, default))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*đang chờ bảo vệ xử lý*");
    }

    // 4. Test trường hợp Thành công (Happy Path)
    [Fact]
    public async Task Handle_Should_CreateRequest_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userMock.Setup(x => x.UserId).Returns(userId);

        var command = new CreateDoorRequestCommand(Guid.NewGuid());
        var expectedEntity = new DoorOpeningRequest { Id = Guid.NewGuid() }; // Entity giả mà Mapper trả về

        // 1. Phòng tồn tại
        _labRepoMock.Setup(x => x.GetByIdAsync(command.LabRoomId, default))
            .ReturnsAsync(new LabRoom { Id = command.LabRoomId });

        // 2. Không spam
        _doorRepoMock.Setup(x => x.HasPendingRequestAsync(userId, command.LabRoomId, default))
            .ReturnsAsync(false);

        // 3. Setup Mapper (Quan trọng vì Handler dùng mapper.Map)
        _mapperMock.Setup(m => m.Map<DoorOpeningRequest>(command))
            .Returns(expectedEntity);

        // Act
        var resultId = await _handler.Handle(command, default);

        // Assert
        // Check ID trả về đúng chưa
        resultId.Should().Be(expectedEntity.Id);

        // Check xem entity đã được gán RequestedById chưa (Logic trong handler)
        expectedEntity.RequestedById.Should().Be(userId);

        // Verify: Check xem hàm CreateAsync của Repo có được gọi đúng 1 lần không
        _doorRepoMock.Verify(x => x.CreateAsync(expectedEntity, default), Times.Once);
    }
}
