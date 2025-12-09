using FluentAssertions;
using LabBooking.Domain.Entities;
using LabBooking.Domain.Enums;
using LabBooking.Infrastructure.Persistence;
using LabBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LabBooking.Infrastructure.UnitTests.Repositories;

public class DoorRequestRepositoryIntegrationTests
{
    private readonly LabBookingDbContext _dbContext;
    private readonly DoorRequestRepository _repository;

    public DoorRequestRepositoryIntegrationTests()
    {
        // Setup DB giả trong RAM
        var options = new DbContextOptionsBuilder<LabBookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new LabBookingDbContext(options);
        _repository = new DoorRequestRepository(_dbContext);
    }

    [Fact]
    public async Task CreateAsync_Should_AddRequestToDatabase()
    {
        // ARRANGE
        var request = new DoorOpeningRequest
        {
            Id = Guid.NewGuid(),
            RequestedById = Guid.NewGuid(),
            LabRoomId = Guid.NewGuid(),
            Status = DoorRequestStatus.Pending,
            Type = DoorRequestType.Open,
            RequestTime = DateTime.UtcNow
        };

        // ACT
        await _repository.CreateAsync(request, CancellationToken.None);

        // ASSERT
        // 1. Kiểm tra số lượng bản ghi trong DB
        var count = await _dbContext.DoorOpeningRequests.CountAsync();
        count.Should().Be(1);

        // 2. Kiểm tra dữ liệu lấy ra có khớp không
        var savedRequest = await _dbContext.DoorOpeningRequests.FirstOrDefaultAsync();
        savedRequest.Should().NotBeNull();
        savedRequest!.Id.Should().Be(request.Id);
        savedRequest.RequestedById.Should().Be(request.RequestedById);
        savedRequest.Status.Should().Be(DoorRequestStatus.Pending);
    }
}
