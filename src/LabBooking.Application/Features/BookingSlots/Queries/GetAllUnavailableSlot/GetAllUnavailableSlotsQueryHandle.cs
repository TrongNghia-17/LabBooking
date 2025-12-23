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
                var (slotTemplates, any) = await slotRepository.GetAllSlotAsync(cancellationToken);
                if (!slotTemplates.Any())
                {
                    return Enumerable.Empty<BookingSlotResponse>();
                }

                // ========== BƯỚC 2: LẤY SLOT ĐÃ ĐẶT (BOOKED) ==========
                // Lưu ý: bookingSlotRepository cần Include: Booking, CreatedBy, Course, Project
                var bookedSlots = await bookingSlotRepository.GetBookedSlotsForRoomAsync(
                    request.LabRoomId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                // Xử lý mapping cho danh sách Booked
                var bookedResponses = new List<BookingSlotResponse>();

                foreach (var slot in bookedSlots)
                {
                    // Map cơ bản bằng AutoMapper
                    var responseItem = mapper.Map<BookingSlotResponse>(slot);

                    // --- LOGIC MỚI: ĐIỀN THÔNG TIN CHI TIẾT ---
                    if (slot.Booking != null)
                    {
                        // 1. Xác định Tiêu đề (Title) dựa trên loại Booking
                        responseItem.Title = GetBookingTitle(slot.Booking);

                        // 2. Tên người đặt
                        responseItem.BookerName = slot.Booking.CreatedBy?.FullName ?? slot.Booking.CreatedBy?.UserName ?? "Unknown";

                        // 3. Mô tả
                        responseItem.Description = slot.Booking.Description;

                        // 4. Loại nhãn
                        responseItem.TypeLabel = GetTypeLabel(slot.Booking.Type);

                        // 5. Check chính chủ
                        responseItem.IsMyBooking = request.CurrentUserId.HasValue &&
                                                   slot.Booking.CreatedById == request.CurrentUserId.Value;
                    }

                    bookedResponses.Add(responseItem);
                }

                // ========== BƯỚC 3: XỬ LÝ LỊCH BẢO TRÌ ==========
                var maintenanceSchedules = await maintainScheduleRepository.GetOverlappingSchedulesAsync(
                    request.LabRoomId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                var maintenanceResponses = new List<BookingSlotResponse>();

                if (maintenanceSchedules.Any())
                {
                    var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                    for (var day = request.StartDate; day <= request.EndDate; day = day.AddDays(1))
                    {
                        foreach (var slot in slotTemplates)
                        {
                            var localSlotStartTime = day.ToDateTime(slot.StartTime);
                            var localSlotEndTime = day.ToDateTime(slot.EndTime);

                            var utcSlotStartTime = TimeZoneInfo.ConvertTime(localSlotStartTime, localTimeZone, TimeZoneInfo.Utc);
                            var utcSlotEndTime = TimeZoneInfo.ConvertTime(localSlotEndTime, localTimeZone, TimeZoneInfo.Utc);

                            // Tìm lịch bảo trì cụ thể gây ra việc trùng giờ
                            var matchingMaintenance = maintenanceSchedules.FirstOrDefault(m =>
                                utcSlotStartTime < m.EndTime &&
                                utcSlotEndTime > m.StartTime
                            );

                            if (matchingMaintenance != null)
                            {
                                // Tạo Response trực tiếp (không qua Entity BookingSlot trung gian để giữ dữ liệu)
                                maintenanceResponses.Add(new BookingSlotResponse
                                {
                                    Id = Guid.NewGuid(), // ID giả cho frontend dùng làm key
                                    Date = day, // DateOnly
                                    SlotId = slot.Id,
                                    // --- DỮ LIỆU QUAN TRỌNG CHO TOOLTIP ---
                                    Reason = "Maintenance",
                                    Priority = 0, // Mức ưu tiên cao nhất

                                    Title = "Bảo trì phòng",
                                    BookerName = "Quản lý phòng",
                                    Description = matchingMaintenance.Description, // Lấy lý do cụ thể (VD: Sửa máy chiếu)
                                    TypeLabel = "Bảo trì",

                                    IsMyBooking = false
                                });
                            }
                        }
                    }
                }

                // ========== BƯỚC 4: KẾT HỢP VÀ LỌC TRÙNG ==========
                // Gộp 2 danh sách lại
                var allResponses = bookedResponses.Concat(maintenanceResponses);

                // Ưu tiên: Nếu 1 slot vừa có Booking vừa có Maintenance -> Lấy Maintenance (Priority 0)
                // Group by Date + SlotId -> Order by Priority (0 trước, 1 sau...) -> Lấy cái đầu tiên
                var finalResult = allResponses
                    .GroupBy(x => new { x.Date, x.SlotId })
                    .Select(g => g.OrderBy(x => x.Priority).First())
                    .ToList();

                logger.LogInformation("Returning {TotalCount} slots (Booked + Maintenance).", finalResult.Count);

                return finalResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling GetUnavailableSlotsQuery");
                throw;
            }
        }

        private string GetBookingTitle(Domain.Entities.Booking booking)
        {
            return booking.Type switch
            {
                BookingType.Teaching => $"{booking.Course?.CourseCode} - {booking.Course?.CourseName}", // VD: CS101 - Java
                BookingType.Project => booking.Project?.ProjectName ?? booking.Title ?? "Dự án nhóm",
                BookingType.UniversityEvent => booking.Title ?? "Sự kiện trường",
                _ => booking.Title ?? "Đã đặt"
            };
        }

        private string GetTypeLabel(BookingType? type)
        {
            return type switch
            {
                BookingType.Teaching => "Lớp học",
                BookingType.Project => "Dự án",
                BookingType.UniversityEvent => "Sự kiện",
                _ => "Hoạt động"
            };
        }
    }
}
