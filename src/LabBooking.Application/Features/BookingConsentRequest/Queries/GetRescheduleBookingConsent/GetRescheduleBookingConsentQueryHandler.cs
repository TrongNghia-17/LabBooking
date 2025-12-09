using LabBooking.Application.Features.BookingConsentRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingConsentRequest.Queries.GetRescheduleBookingConsent
{
    internal class GetRescheduleBookingConsentQueryHandler(IBookingConsentRequestRepository consentRepository)
        : IRequestHandler<GetRescheduleBookingConsentQuery, RescheduleResponse>
    {
        public async Task<RescheduleResponse> Handle(GetRescheduleBookingConsentQuery request, CancellationToken cancellationToken)
        {
            // GỌI REPO: Thay vì dùng dbContext trực tiếp
            var consent = await consentRepository.GetByIdWithBookingAndSlotsAsync(request.ConsentId, cancellationToken);

            if (consent == null)
                throw new KeyNotFoundException("Không tìm thấy yêu cầu xác nhận.");

            // Parse danh sách slot bị đè
            List<Guid> overriddenIds = new();

            if (!string.IsNullOrWhiteSpace(consent.OverriddenSlotIdsJson))
            {
                try
                {
                    // 1. Deserialize ra danh sách Object trước
                    var jsonObjects = JsonSerializer.Deserialize<List<OverriddenSlotJsonModel>>(consent.OverriddenSlotIdsJson);

                    // 2. Lấy ra list ID (BookingSlotId) từ danh sách Object đó
                    if (jsonObjects != null)
                    {
                        // Lưu ý: Kiểm tra xem logic của bạn đang so sánh với BookingSlot.Id (GUID của dòng trong bảng BookingSlot)
                        // Hay là SlotId (GUID của bảng SlotTemplate).
                        // Dựa vào code cũ "slot.Id" (trong Booking.Slots), tôi đoán bạn cần BookingSlotId.
                        overriddenIds = jsonObjects.Select(x => x.BookingSlotId).ToList();
                    }
                }
                catch (JsonException)
                {
                    // Fallback: Phòng trường hợp có dữ liệu cũ lưu dạng ["guid1", "guid2"]
                    try
                    {
                        overriddenIds = JsonSerializer.Deserialize<List<Guid>>(consent.OverriddenSlotIdsJson) ?? new();
                    }
                    catch
                    {
                        overriddenIds = new(); // Data lỗi hẳn thì trả về rỗng
                    }
                }
            }

            var context = new RescheduleResponse
            {
                BookingId = consent.BookingId,
                BookingTitle = consent.Booking.Title ?? "Unknown Booking",
                LabId = consent.Booking.LabRoomId,
                LabName = consent.Booking.LabRoom?.LabName ?? "Unknown Lab",
                SlotDebtCount = overriddenIds.Count
            };

            // Phân loại Slot
            if (consent.Booking.Slots != null)
            {
                foreach (var slot in consent.Booking.Slots)
                {
                    var simpleSlot = new SimpleSlotDto
                    {
                        SlotId = slot.SlotId,
                        Date = slot.Date.ToString("yyyy-MM-dd")
                    };

                    if (overriddenIds.Contains(slot.Id))
                    {
                        context.LostSlots.Add(simpleSlot);
                    }
                    else if (slot.Status == BookingSlotStatus.Active)
                    {
                        context.SafeSlots.Add(simpleSlot);
                    }
                }
            }

            return context;
        }
    }
}
