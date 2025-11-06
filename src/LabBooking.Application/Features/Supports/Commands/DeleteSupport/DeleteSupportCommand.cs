namespace LabBooking.Application.Features.Supports.Commands.DeleteSupport;

/// <summary>
/// Command để xử lý logic xóa một support ticket.
/// Sử dụng IRequest<Unit> vì không cần trả về dữ liệu gì sau khi xóa.
/// </summary>
public record DeleteSupportCommand(Guid Id) : IRequest<Unit>;
