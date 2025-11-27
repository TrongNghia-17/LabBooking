using AutoMapper;
using FluentAssertions;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;
using LabBooking.Domain.Entities;
using LabBooking.Domain.Exceptions;
using LabBooking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace LabBooking.Application.UnitTests.Features.EquipmentMaintainSchedules;

public class CreateEquipmentMaintainScheduleCommandHandlerTests
{
    // Khai báo các Mock (đối tượng giả lập)
    private readonly Mock<IEquipmentMaintainScheduleRepository> _mockScheduleRepo;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepo;
    private readonly Mock<ILabRoomRepository> _mockLabRoomRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>> _mockLogger;

    // Đối tượng cần test (System Under Test)
    private readonly CreateEquipmentMaintainScheduleCommandHandler _handler;

    public CreateEquipmentMaintainScheduleCommandHandlerTests()
    {
        // 1. Khởi tạo các Mock
        _mockScheduleRepo = new Mock<IEquipmentMaintainScheduleRepository>();
        _mockEquipmentRepo = new Mock<IEquipmentRepository>();
        _mockLabRoomRepo = new Mock<ILabRoomRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>>();

        // 2. Tiêm các Mock vào Handler thật
        _handler = new CreateEquipmentMaintainScheduleCommandHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockScheduleRepo.Object,
            _mockEquipmentRepo.Object,
            _mockLabRoomRepo.Object,
            _mockCurrentUserService.Object
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnResponse_When_UserIsManagerOfLab()
    {
        // --- ARRANGE (Chuẩn bị dữ liệu giả) ---
        var userId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: equipmentId,
            StartTime: DateTime.UtcNow.AddDays(1),
            EndTime: DateTime.UtcNow.AddDays(1).AddHours(2),
            Description: "Bảo trì định kỳ"
        );

        // Giả lập user đã đăng nhập
        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        // Giả lập tìm thấy thiết bị, thiết bị này thuộc phòng labRoomId
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        // Giả lập tìm thấy phòng Lab, và MainManagerId CHÍNH LÀ user đang đăng nhập
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        // Giả lập Mapper trả về entity và response
        var scheduleEntity = new EquipmentMaintainSchedule { Id = Guid.NewGuid() };
        var scheduleResponse = new EquipmentMaintainScheduleResponse { Id = scheduleEntity.Id };

        _mockMapper.Setup(m => m.Map<EquipmentMaintainSchedule>(command)).Returns(scheduleEntity);
        _mockMapper.Setup(m => m.Map<EquipmentMaintainScheduleResponse>(scheduleEntity)).Returns(scheduleResponse);

        // --- ACT (Thực thi) ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- ASSERT (Kiểm tra kết quả) ---
        result.Should().NotBeNull();
        result.Id.Should().Be(scheduleResponse.Id);

        // Kiểm tra xem hàm Create của Repository có được gọi 1 lần không
        _mockScheduleRepo.Verify(x => x.Create(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserNotLoggedIn()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns((Guid?)null); // User null
        var command = new CreateEquipmentMaintainScheduleCommand(Guid.NewGuid(), DateTime.Now, DateTime.Now, "Test");

        // ACT
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Bạn cần đăng nhập*");
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_When_EquipmentDoesNotExist()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        // Repo trả về null (không tìm thấy thiết bị)
        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Equipment?)null);

        var command = new CreateEquipmentMaintainScheduleCommand(Guid.NewGuid(), DateTime.Now, DateTime.Now, "Test");

        // ACT 
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowForbid_When_UserIsNotManager()
    {
        // ARRANGE
        var currentUserId = Guid.NewGuid();
        var otherManagerId = Guid.NewGuid(); // ID người khác
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(currentUserId);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        // Phòng Lab có MainManager là người khác
        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = otherManagerId });

        var command = new CreateEquipmentMaintainScheduleCommand(equipmentId, DateTime.Now, DateTime.Now, "Test");

        // ACT
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<ForbidException>()
            .WithMessage("Bạn không có quyền*");
    }
}
