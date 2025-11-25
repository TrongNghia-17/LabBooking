namespace LabBooking.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking newBooking);
        Task<List<(Booking Booking, bool HasPendingRequest)>> GetBookingsWithChangeStatusAsync(Guid userId)
        Task<Booking?> GetBookingDetailsAsync(Guid id);
        Task<List<Booking>> GetPendingBookingsAsync(Guid? labId);

        //Task<BookingApprovalResult> ApproveBookingAsync(Guid bookingId, Guid approverId);
        Task<Booking?> GetBookingByIdWithSlotsAsync(Guid id);
        Task<List<BookingSlot>> GetConflictingSlotsAsync(Guid labRoomId, List<BookingSlot> requestedSlots, Guid? excludeBookingId = null);

        // Hàm "Thần thánh": Xử lý toàn bộ logic Duyệt + Đè + Tạo Consent
        Task ApproveBookingWithOverrideLogicAsync(Booking booking);
        Task<List<Guid>> GetBookedLabIdsAsync(DateOnly date, Guid slotId, CancellationToken ct);
        Task<List<Booking>> GetHistoryByUserIdAsync(Guid userId);
    }

    public enum BookingApprovalResult
    {
        Success,            // Đã duyệt thành công
        WaitingForConsent,  // Đã gửi yêu cầu thỏa thuận
        Failed              // Lỗi khác (nếu cần)
    }
}
