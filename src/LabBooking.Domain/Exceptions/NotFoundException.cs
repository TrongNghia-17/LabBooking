namespace LabBooking.Domain.Exceptions;

public class NotFoundException : Exception
{
    // Constructor cũ (giữ nguyên để không lỗi code cũ)
    public NotFoundException(string resourceType, string resourceIdentifier)
        : base($"{resourceType} with id: {resourceIdentifier} doesn't exist")
    {
    }

    // --- THÊM CONSTRUCTOR MỚI NÀY ---
    // Cho phép truyền message tùy chỉnh (Tiếng Việt)
    public NotFoundException(string message) : base(message)
    {
    }
}