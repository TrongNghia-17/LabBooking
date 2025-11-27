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
    private readonly Mock<IEquipmentMaintainScheduleRepository> _mockScheduleRepo;
    private readonly Mock<IEquipmentRepository> _mockEquipmentRepo;
    private readonly Mock<ILabRoomRepository> _mockLabRoomRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>> _mockLogger;

    private readonly CreateEquipmentMaintainScheduleCommandHandler _handler;

    public CreateEquipmentMaintainScheduleCommandHandlerTests()
    {
        _mockScheduleRepo = new Mock<IEquipmentMaintainScheduleRepository>();
        _mockEquipmentRepo = new Mock<IEquipmentRepository>();
        _mockLabRoomRepo = new Mock<ILabRoomRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CreateEquipmentMaintainScheduleCommandHandler>>();

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
        // --- ARRANGE ---
        var userId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        var command = new CreateEquipmentMaintainScheduleCommand(
            EquipmentId: equipmentId,
            StartTime: DateTime.UtcNow.AddDays(1),
            EndTime: DateTime.UtcNow.AddDays(1).AddHours(2),
            Description: "Bảo trì định kỳ"
        );

        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

        _mockLabRoomRepo.Setup(x => x.GetByIdAsync(labRoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LabRoom { Id = labRoomId, MainManagerId = userId });

        var scheduleEntity = new EquipmentMaintainSchedule { Id = Guid.NewGuid() };
        var scheduleResponse = new EquipmentMaintainScheduleResponse { Id = scheduleEntity.Id };

        _mockMapper.Setup(m => m.Map<EquipmentMaintainSchedule>(command)).Returns(scheduleEntity);
        _mockMapper.Setup(m => m.Map<EquipmentMaintainScheduleResponse>(scheduleEntity)).Returns(scheduleResponse);

        // --- ACT ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- ASSERT ---
        result.Should().NotBeNull();
        result.Id.Should().Be(scheduleResponse.Id);

        _mockScheduleRepo.Verify(x => x.Create(It.IsAny<EquipmentMaintainSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserNotLoggedIn()
    {
        // ARRANGE
        _mockCurrentUserService.Setup(x => x.UserId).Returns((Guid?)null);
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
        var otherManagerId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var labRoomId = Guid.NewGuid();

        _mockCurrentUserService.Setup(x => x.UserId).Returns(currentUserId);

        _mockEquipmentRepo.Setup(x => x.GetByIdAsync(equipmentId))
            .ReturnsAsync(new Equipment { Id = equipmentId, LabRoomId = labRoomId });

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
