using LabBooking.Application.Features.BookingSlots.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Queries.GetAllUnavailableSlot
{
    public class GetAllUnavailableSlotsQueryHandle(
        ILogger<GetAllUnavailableSlotsQueryHandle> logger,
        IBookingSlotRepository bookingSlotRepository,
        ISlotRepository slotRepository,
        IRoomMaintainScheduleRepository maintainScheduleRepository,
        IMapper mapper)
        : IRequestHandler<GetAllUnavailableSlotsQuery, IEnumerable<BookingSlotResponse>>
    {
        public async Task<IEnumerable<BookingSlotResponse>> Handle(GetAllUnavailableSlotsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetUnavailableSlotsQuery for RoomId: {LabRoomId} from {StartDate} to {EndDate}",
                request.LabRoomId, request.StartDate, request.EndDate);

            try
            {
                // ========== BƯỚC 1: LẤY SLOT MẪU ==========
                // Lấy 4 slot mẫu (7h-9h, 9h-11h, v.v.)
                var (slotTemplates, any) = (await slotRepository.GetAllSlotAsync(cancellationToken));
                if (!slotTemplates.Any())
                {
                    logger.LogWarning("No slot templates found in database.");
                    return Enumerable.Empty<BookingSlotResponse>();
                }

                // ========== BƯỚC 2: LẤY SLOT ĐÃ ĐẶT (BOOKED) ==========
                // (Giả sử Repository có hàm này)
                var bookedSlots = await bookingSlotRepository.GetBookedSlotsForRoomAsync(
                    request.LabRoomId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                // ========== BƯỚC 3A: LẤY LỊCH BẢO TRÌ ==========
                // Lấy các lịch bảo trì CÓ CHỒNG CHÉO (overlap) với tuần đang xem
                var maintenanceSchedules = await maintainScheduleRepository.GetOverlappingSchedulesAsync(
                    request.LabRoomId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                var maintenanceSlotList = new List<BookingSlot>();

                if (maintenanceSchedules.Any())
                {
                    // --- SỬA LỖI Ở ĐÂY ---

                    // 1. Định nghĩa múi giờ Local (Việt Nam)
                    var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                    // ========== BƯỚC 3B: CHUYỂN BẢO TRÌ THÀNH SLOT ==========
                    for (var day = request.StartDate; day <= request.EndDate; day = day.AddDays(1))
                    {
                        foreach (var slot in slotTemplates)
                        {
                            // 2. Tạo DateTime cục bộ (Kind=Unspecified)
                            // Dùng ToDateTime(TimeOnly) cho sạch
                            var localSlotStartTime = day.ToDateTime(slot.StartTime);
                            var localSlotEndTime = day.ToDateTime(slot.EndTime);

                            // 3. Chuyển đổi thời gian cục bộ này sang UTC
                            var utcSlotStartTime = TimeZoneInfo.ConvertTime(localSlotStartTime, localTimeZone, TimeZoneInfo.Utc);
                            var utcSlotEndTime = TimeZoneInfo.ConvertTime(localSlotEndTime, localTimeZone, TimeZoneInfo.Utc);

                            // 4. KIỂM TRA CHỒNG CHÉO (UTC vs UTC)
                            // (m.EndTime và m.StartTime đã là UTC từ DB)
                            bool isUnderMaintenance = maintenanceSchedules.Any(m =>
                                utcSlotStartTime < m.EndTime && // Slot bắt đầu TRƯỚC khi bảo trì kết thúc
                                utcSlotEndTime > m.StartTime    // Slot kết thúc SAU khi bảo trì bắt đầu
                            );

                            if (isUnderMaintenance)
                            {
                                // 5. Tạo "BookingSlot giả"
                                maintenanceSlotList.Add(new BookingSlot
                                {
                                    Id = Guid.NewGuid(),
                                    Date = day, // Date là DateOnly, gán thẳng
                                    SlotId = slot.Id,
                                    Slot = slot,
                                    Reason = UnavailableReason.Maintenance,
                                    Priority = 0
                                });
                            }
                        }
                    }
                    // --- KẾT THÚC SỬA ---
                }

                // ========== BƯỚC 4: KẾT HỢP VÀ TRẢ VỀ ==========
                var combinedSlots = bookedSlots.Concat(maintenanceSlotList);

                // Lọc trùng lặp (phòng trường hợp 1 slot vừa bị đặt vừa bị bảo trì)
                // Phải so sánh .Date.Date vì `Date` trong BookingSlot là DateTime
                var distinctSlots = combinedSlots
                    .DistinctBy(s => new { s.Date, s.SlotId });

                logger.LogInformation("Found {BookedCount} booked slots and {MaintenanceCount} maintenance slots. Returning {TotalCount} unavailable slots.",
                    bookedSlots.Count(), maintenanceSlotList.Count, distinctSlots.Count());

                // Dùng AutoMapper để chuyển đổi sang DTO
                return mapper.Map<IEnumerable<BookingSlotResponse>>(distinctSlots);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while handling GetUnavailableSlotsQuery for RoomId: {LabRoomId}", request.LabRoomId);
                throw;
            }
        }
    }
}
