using AutoMapper;
using FluentAssertions;
using LabBooking.Application.Features.EquipmentCategories.Dtos;
using LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;
using LabBooking.Domain.Entities;
using LabBooking.Domain.Repositories;
using Moq;

namespace LabBooking.Application.UnitTests.Features.EquipmentCategories;

public class GetAllEquipmentCategoriesQueryHandlerTests
{
    private readonly Mock<IEquipmentCategoryRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllEquipmentCategoriesQueryHandler _handler;

    public GetAllEquipmentCategoriesQueryHandlerTests()
    {
        _mockRepo = new Mock<IEquipmentCategoryRepository>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetAllEquipmentCategoriesQueryHandler(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnListOfCategories_When_DataExists()
    {
        // --- ARRANGE (Chuẩn bị) ---
        var categoriesFromDb = new List<EquipmentCategory>
        {
            new() { Id = Guid.NewGuid(), Name = "IoT", Equipments = new List<Equipment>() }, // Count = 0
            new() { Id = Guid.NewGuid(), Name = "Laptop", Equipments = new List<Equipment> { new(), new() } } // Count = 2
        };

        var categoriesResponse = new List<EquipmentCategoryResponse>
        {
            new() { Id = categoriesFromDb[0].Id, Name = "IoT", EquipmentCount = 0 },
            new() { Id = categoriesFromDb[1].Id, Name = "Laptop", EquipmentCount = 2 }
        };

        _mockRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoriesFromDb);

        _mockMapper.Setup(x => x.Map<IEnumerable<EquipmentCategoryResponse>>(categoriesFromDb))
            .Returns(categoriesResponse);

        var query = new GetAllEquipmentCategoriesQuery();

        // --- ACT (Thực thi) ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- ASSERT (Kiểm tra) ---
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("IoT");

        _mockRepo.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoDataExists()
    {
        // --- ARRANGE ---
        var emptyList = new List<EquipmentCategory>();

        _mockRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        _mockMapper.Setup(x => x.Map<IEnumerable<EquipmentCategoryResponse>>(emptyList))
            .Returns(new List<EquipmentCategoryResponse>());

        // --- ACT ---
        var result = await _handler.Handle(new GetAllEquipmentCategoriesQuery(), CancellationToken.None);

        // --- ASSERT ---
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
