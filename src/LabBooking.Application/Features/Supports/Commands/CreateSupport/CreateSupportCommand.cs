using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

/// <summary>
/// Record chứa dữ liệu đầu vào để tạo một support ticket mới.
/// </summary>
/// <param name="Title">Tiêu đề của support ticket.</param>
/// <param name="Content">Nội dung chi tiết của support ticket.</param>
/// <param name="CreatedById">ID của người dùng tạo support ticket.</param>
public record CreateSupportCommand(
    string Title,
    string Content,
    Guid CreatedById
) : IRequest<SupportsResponse>;
