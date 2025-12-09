namespace LabBooking.Application.Features.EquipmentCategories.Commands.Update;

public class UpdateEquipmentCategoryCommandHandler(
    IEquipmentCategoryRepository repository,
    ILogger<UpdateEquipmentCategoryCommandHandler> logger
    ) : IRequestHandler<UpdateEquipmentCategoryCommand>
{
    public async Task Handle(UpdateEquipmentCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy dữ liệu cũ từ DB
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra tồn tại
        if (category == null)
        {
            throw new NotFoundException(nameof(EquipmentCategory), request.Id.ToString());
        }

        // 3. Cập nhật thông tin
        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();

        // 4. Lưu xuống DB
        await repository.UpdateAsync(category, cancellationToken);

        logger.LogInformation("Updated Equipment Category: {Id} - {Name}", request.Id, request.Name);
    }
}