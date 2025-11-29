using FluentAssertions;
using LabBooking.Domain.Entities;
using LabBooking.Infrastructure.Persistence;
using LabBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LabBooking.Infrastructure.UnitTests.Repositories;

public class EquipmentMaintainScheduleRepositoryTests
{
    private readonly LabBookingDbContext _dbContext;
    private readonly EquipmentMaintainScheduleRepository _repository;

    public EquipmentMaintainScheduleRepositoryTests()
    {
        // 1. Cấu hình In-Memory Database (Tạo DB riêng cho mỗi lần chạy test để không bị trùng data)
        var options = new DbContextOptionsBuilder<LabBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new LabBookingDbContext(options);

        // 2. Khởi tạo Repository với DB giả
        _repository = new EquipmentMaintainScheduleRepository(_dbContext);
    }

    // --- TEST CASE 1: BẮT ĐẦU BẢO TRÌ (START MAINTENANCE) ---
    [Fact]
    public async Task ProcessAutoStatusUpdatesAsync_Should_SetStatusToMaintain_When_TimeHasCome()
    {
        // ARRANGE
        var equipmentId = Guid.NewGuid();

        // Tạo thiết bị đang Sẵn sàng
        var equipment = new Equipment
        {
            Id = equipmentId,
            Status = EquipmentStatus.Available,
            IsAvailable = true,
            EquipmentName = "Test PC"
        };

        // Tạo lịch đã đến giờ bắt đầu (StartTime < Now < EndTime)
        var schedule = new EquipmentMaintainSchedule
        {
            Id = Guid.NewGuid(),
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow.AddMinutes(-10), // Đã bắt đầu 10 phút trước
            EndTime = DateTime.UtcNow.AddMinutes(50),    // Còn 50 phút nữa mới xong
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };

        // Lưu vào DB giả
        await _dbContext.Equipments.AddAsync(equipment);
        await _dbContext.EquipmentMaintainSchedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync();

        // ACT
        var result = await _repository.ProcessAutoStatusUpdatesAsync();

        // ASSERT
        // 1. Kiểm tra return message
        result.Should().Contain("Đã chuyển 1 thiết bị sang 'Bảo trì'");

        // 2. Kiểm tra lại DB xem thiết bị đã đổi trạng thái chưa
        var updatedEquipment = await _dbContext.Equipments.FindAsync(equipmentId);
        updatedEquipment!.Status.Should().Be(EquipmentStatus.Maintain);
        updatedEquipment.IsAvailable.Should().BeFalse();
    }

    // --- TEST CASE 2: KẾT THÚC BẢO TRÌ (END MAINTENANCE) ---
    [Fact]
    public async Task ProcessAutoStatusUpdatesAsync_Should_SetStatusToAvailable_And_MarkDone_When_TimeIsOver()
    {
        // ARRANGE
        var equipmentId = Guid.NewGuid();

        // Tạo thiết bị đang Bảo trì
        var equipment = new Equipment
        {
            Id = equipmentId,
            Status = EquipmentStatus.Maintain,
            IsAvailable = false
        };

        // Tạo lịch đã hết giờ (EndTime < Now)
        var schedule = new EquipmentMaintainSchedule
        {
            Id = Guid.NewGuid(),
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow.AddMinutes(-60),
            EndTime = DateTime.UtcNow.AddMinutes(-10), // Đã kết thúc 10 phút trước
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet // Chưa được mark Done
        };

        await _dbContext.Equipments.AddAsync(equipment);
        await _dbContext.EquipmentMaintainSchedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync();

        // ACT
        var result = await _repository.ProcessAutoStatusUpdatesAsync();

        // ASSERT
        // 1. Kiểm tra message
        result.Should().Contain("Đã hoàn tất 1 lịch bảo trì");

        // 2. Kiểm tra thiết bị đã về Available chưa
        var updatedEquipment = await _dbContext.Equipments.FindAsync(equipmentId);
        updatedEquipment!.Status.Should().Be(EquipmentStatus.Available);
        updatedEquipment.IsAvailable.Should().BeTrue();

        // 3. Kiểm tra lịch đã chuyển sang Done chưa
        var updatedSchedule = await _dbContext.EquipmentMaintainSchedules.FindAsync(schedule.Id);
        updatedSchedule!.EquimentpMaintainStatus.Should().Be(EquimentpMaintainStatus.Done);
    }

    // --- TEST CASE 3: KHÔNG ĐỔI TRẠNG THÁI NẾU THIẾT BỊ HỎNG (BROKEN) ---
    // Đây là logic an toàn quan trọng: Lịch xong nhưng máy vẫn hỏng thì không được mở lại.
    [Fact]
    public async Task ProcessAutoStatusUpdatesAsync_Should_NotChangeStatus_If_EquipmentIsBroken()
    {
        // ARRANGE
        var equipmentId = Guid.NewGuid();

        // Thiết bị đang Hỏng (Broken)
        var equipment = new Equipment
        {
            Id = equipmentId,
            Status = EquipmentStatus.Broken, // <--- Quan trọng
            IsAvailable = false
        };

        // Lịch đã kết thúc
        var schedule = new EquipmentMaintainSchedule
        {
            Id = Guid.NewGuid(),
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow.AddMinutes(-60),
            EndTime = DateTime.UtcNow.AddMinutes(-10),
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };

        await _dbContext.Equipments.AddAsync(equipment);
        await _dbContext.EquipmentMaintainSchedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync();

        // ACT
        await _repository.ProcessAutoStatusUpdatesAsync();

        // ASSERT
        var updatedEquipment = await _dbContext.Equipments.FindAsync(equipmentId);

        // Status vẫn phải là Broken (Không được tự động chuyển về Available)
        updatedEquipment!.Status.Should().Be(EquipmentStatus.Broken);
        updatedEquipment.IsAvailable.Should().BeFalse();

        // Lịch vẫn được mark Done (vì đã hết giờ)
        var updatedSchedule = await _dbContext.EquipmentMaintainSchedules.FindAsync(schedule.Id);
        updatedSchedule!.EquimentpMaintainStatus.Should().Be(EquimentpMaintainStatus.Done);
    }

    // --- TEST CASE 4: KHÔNG LÀM GÌ NẾU CHƯA ĐẾN GIỜ ---
    [Fact]
    public async Task ProcessAutoStatusUpdatesAsync_Should_DoNothing_When_ScheduleIsInFuture()
    {
        // ARRANGE
        var equipmentId = Guid.NewGuid();
        var equipment = new Equipment { Id = equipmentId, Status = EquipmentStatus.Available };

        // Lịch ở tương lai (StartTime > Now)
        var schedule = new EquipmentMaintainSchedule
        {
            Id = Guid.NewGuid(),
            EquipmentId = equipmentId,
            StartTime = DateTime.UtcNow.AddMinutes(10), // Tương lai 10p
            EndTime = DateTime.UtcNow.AddMinutes(70),
            EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet
        };

        await _dbContext.Equipments.AddAsync(equipment);
        await _dbContext.EquipmentMaintainSchedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync();

        // ACT
        var result = await _repository.ProcessAutoStatusUpdatesAsync();

        // ASSERT
        result.Should().Contain("Đã chuyển 0");
        result.Should().Contain("Đã hoàn tất 0");

        var updatedEquipment = await _dbContext.Equipments.FindAsync(equipmentId);
        updatedEquipment!.Status.Should().Be(EquipmentStatus.Available); // Không đổi
    }
}
