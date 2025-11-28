using AutoMapper;
using FluentAssertions;
using LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;
using LabBooking.Application.Features.Equipments.Dtos;
using LabBooking.Domain.Entities;
using LabBooking.Domain.Repositories;
using Moq;

namespace LabBooking.Application.UnitTests.Features.EquipmentCategories;

public class GetEquipmentsByCategoryQueryHandlerTests
{
    private readonly Mock<IEquipmentCategoryRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetEquipmentsByCategoryQueryHandler _handler;

    public GetEquipmentsByCategoryQueryHandlerTests()
    {
        _mockRepo = new Mock<IEquipmentCategoryRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetEquipmentsByCategoryQueryHandler(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnEquipments_When_CategoryHasEquipments()
    {
        // --- ARRANGE ---
        var categoryId = Guid.NewGuid();

        var equipmentsFromDb = new List<Equipment>
        {
            new() { Id = Guid.NewGuid(), EquipmentName = "Sensor A", EquipmentCategoryId = categoryId, Status = EquipmentStatus.Available },
            new() { Id = Guid.NewGuid(), EquipmentName = "Sensor B", EquipmentCategoryId = categoryId, Status = EquipmentStatus.Broken }
        };

        var responseDtos = new List<EquipmentSimpleResponse>
        {
            new() { Id = equipmentsFromDb[0].Id, EquipmentName = "Sensor A", Status = "Sẵn sàng" },
            new() { Id = equipmentsFromDb[1].Id, EquipmentName = "Sensor B", Status = "Hỏng" }
        };

        // 3. Setup Mock
        _mockRepo.Setup(x => x.GetEquipmentsByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipmentsFromDb);

        _mockMapper.Setup(x => x.Map<IEnumerable<EquipmentSimpleResponse>>(equipmentsFromDb))
            .Returns(responseDtos);

        var query = new GetEquipmentsByCategoryQuery(categoryId);

        // --- ACT ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- ASSERT ---
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().EquipmentName.Should().Be("Sensor A");

        _mockRepo.Verify(x => x.GetEquipmentsByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_When_CategoryHasNoEquipments()
    {
        // --- ARRANGE ---
        var categoryId = Guid.NewGuid();
        var emptyList = new List<Equipment>();

        _mockRepo.Setup(x => x.GetEquipmentsByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        _mockMapper.Setup(x => x.Map<IEnumerable<EquipmentSimpleResponse>>(emptyList))
            .Returns(new List<EquipmentSimpleResponse>());

        // --- ACT ---
        var result = await _handler.Handle(new GetEquipmentsByCategoryQuery(categoryId), CancellationToken.None);

        // --- ASSERT ---
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
