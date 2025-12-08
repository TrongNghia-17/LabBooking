namespace LabBooking.Application.Features.EquipmentCategories.Commands.Create;

public class CreateEquipmentCategoryCommandHandler(
    IEquipmentCategoryRepository repository,
    IMapper mapper,
    ILogger<CreateEquipmentCategoryCommandHandler> logger
    ) : IRequestHandler<CreateEquipmentCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateEquipmentCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new Equipment Category: {Name}", request.Name);

        // Map từ Command sang Entity
        // Bạn có thể dùng AutoMapper hoặc gán tay như dưới đây
        var category = new EquipmentCategory
        {
            // Id được new tự động trong Entity rồi, hoặc gán thủ công nếu muốn
            // Id = Guid.NewGuid(), 
            Name = request.Name.Trim(), // Nên Trim() để xóa khoảng trắng thừa
            Description = request.Description?.Trim()
        };

        // Lưu vào DB
        var id = await repository.CreateAsync(category, cancellationToken);

        logger.LogInformation("Equipment Category created successfully with ID: {Id}", id);

        return id;
    }
}