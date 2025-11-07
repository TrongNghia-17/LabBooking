namespace LabBooking.Application.Services.Users;

public interface ICurrentUserService
{
    /// <summary>
    /// Lấy UserId (Guid) của người dùng đang đăng nhập từ token.
    /// Trả về null nếu không thể tìm thấy hoặc không thể parse.
    /// </summary>
    Guid? UserId { get; }
}
