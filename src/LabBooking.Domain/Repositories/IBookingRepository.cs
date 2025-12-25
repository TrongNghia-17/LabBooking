namespace LabBooking.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<bool> CheckBookingIsBelongToThisManager(Guid bookingId, Guid managerId);
        Task<Booking> CreateBookingAsync(Booking newBooking);
        Task<List<(Booking Booking, bool HasPendingRequest)>> GetBookingsWithChangeStatusAsync(Guid userId);
        Task<Booking?> GetBookingDetailsAsync(Guid id);
        Task<List<Booking>> GetPendingBookingsAsync(Guid? userId);

        Task<List<Booking>> GetHistoryBookingsAsync(Guid? userId);
        //Task<BookingApprovalResult> ApproveBookingAsync(Guid bookingId, Guid approverId);
        Task<Booking?> GetBookingByIdWithSlotsAsync(Guid id);
        Task<List<BookingSlot>> GetConflictingSlotsAsync(Guid labRoomId, List<BookingSlot> requestedSlots, Guid? excludeBookingId = null);

        // Hàm "Thần thánh": Xử lý toàn bộ logic Duyệt + Đè + Tạo Consent
        Task ApproveBookingWithOverrideLogicAsync(Booking booking);
        Task RejectBookingAsync(Guid bookingId, Guid managerId, string? reason);
        Task<List<Guid>> GetBookedLabIdsAsync(DateOnly date, Guid slotId, CancellationToken ct);
        Task<List<Booking>> GetHistoryByUserIdAsync(Guid userId);
        Task<List<Booking>> GetApprovedHistoryByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        //------NghiaHT-------//

        // Tìm Booking để check tồn tại và lấy ManagerId
        Task<(bool Exists, Guid? ManagerId)> GetBookingAndManagerInfoAsync(string bookingCode);

        // Hoặc kiểm tra xem User hiện tại có sở hữu Booking này không (bảo mật)
        Task<bool> IsBookingOwnedByUserAsync(string bookingCode, Guid userId);
        Task<Booking?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    }

    public enum BookingApprovalResult
    {
        Success,            // Đã duyệt thành công
        WaitingForConsent,  // Đã gửi yêu cầu thỏa thuận
        Failed              // Lỗi khác (nếu cần)
    }
}
