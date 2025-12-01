using LabBooking.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingChangeRequestRepository(LabBookingDbContext dbContext) : IBookingChangeRequestRepository
    {
        public async Task<BookingChangeRequest> CreateAsync(BookingChangeRequest entity)
        {
            dbContext.BookingChangeRequests.Add(entity);
            await dbContext.SaveChangesAsync();
            return await dbContext.BookingChangeRequests
                .Include(r => r.NewSlots) // Load slot mới
                .Include(r => r.Booking)  // Load booking gốc
                .ThenInclude(b => b.LabRoom) // Load phòng từ booking gốc
                .FirstOrDefaultAsync(r => r.Id == entity.Id);
        }

        public async Task<bool> IsBookingOwnerAndApprovedAsync(Guid bookingId, Guid userId)
        {
            // Logic:
            // 1. Phải đúng BookingId
            // 2. Phải đúng người tạo (CreatedById == userId)
            // 3. (Tùy chọn) Booking phải đang Approved mới được làm đơn xin đổi

            return await dbContext.Bookings.AnyAsync(b =>
                b.Id == bookingId &&
                b.CreatedById == userId &&
                b.Status == BookingStatus.Approved
            );
        }

        public async Task<List<BookingChangeRequest>> GetPendingRequestsAsync(Guid? userId)
        {
            var query = dbContext.BookingChangeRequests
                .Include(r => r.NewSlots)        // Lấy danh sách slot mới
                .Include(r => r.Booking)         // [QUAN TRỌNG] Join sang Booking gốc
                    .ThenInclude(b => b.LabRoom) // Lấy thông tin phòng để hiển thị/lọc
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Slots)  // Lấy Course cũ (nếu cần hiển thị)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.ExternalEquipments)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.OutSideGuests)
                .Where(r => r.Status == BookingChangeRequestStatus.Pending);

            // Lọc theo LabId (dựa vào Booking gốc)
            if (userId.HasValue)
            {
                query = query.Where(r => r.Booking.LabRoom.MainManagerId == userId);
            }

            // Sắp xếp: Đơn cũ nhất lên đầu (FIFO) để duyệt trước
            return await query
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task RejectChangeRequestAsync(Guid requestId, Guid managerId)
        {
            // 1. Tìm Request
            var request = await dbContext.BookingChangeRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new BadRequestException("Không tìm thấy yêu cầu thay đổi.");

            if (request.Status != BookingChangeRequestStatus.Pending)
                throw new BadRequestException("Yêu cầu này không còn ở trạng thái chờ.");

            // 2. Update trạng thái
            request.Status = BookingChangeRequestStatus.Rejected;
            // request.RejectedById = managerId; // (Optional)

            // 3. Save
            //await dbContext.SaveChangesAsync();
        }

        public Task<bool> CheckBookingChangeRequestIsBelongToThisManager(Guid bookingChangeId, Guid managerId)
        {
            return dbContext.BookingChangeRequests
                .Include(r => r.Booking)
                .AnyAsync(r => r.Id == bookingChangeId && r.Booking.LabRoom.MainManagerId == managerId);
        }

        public async Task<BookingChangeRequest?> GetRequestWithDetailsAsync(Guid requestId)
        {
            return await dbContext.BookingChangeRequests
                .Include(r => r.NewSlots)
                .Include(r => r.Booking) // Load Booking gốc để lấy LabId check trùng
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        public async Task ApproveRequestAsync(Guid requestId, Guid managerId)
        {
            // 1. LẤY REQUEST & BOOKING GỐC
            var request = await GetRequestWithDetailsAsync(requestId);
            if (request == null)
                throw new BadRequestException("Không tìm thấy yêu cầu thay đổi.");

            if (request.Status != BookingChangeRequestStatus.Pending)
                throw new BadRequestException("Yêu cầu này không còn ở trạng thái chờ.");

            var booking = request.Booking;

            // Load thông tin phòng để check sức chứa
            await dbContext.Entry(booking).Reference(b => b.LabRoom).LoadAsync();

            // =========================================================
            // [FIX 1] KIỂM TRA SỨC CHỨA (CAPACITY)
            // =========================================================
            if (request.NewNumberOfParticipants.HasValue)
            {
                if (request.NewNumberOfParticipants.Value > booking.LabRoom.MaximumLimit)
                {
                    throw new BadRequestException($"Số lượng người tham gia ({request.NewNumberOfParticipants}) vượt quá sức chứa của phòng ({booking.LabRoom.MaximumLimit}).");
                }
            }

            // =========================================================
            // [FIX 2, 3, 4] KIỂM TRA LỊCH (QUÁ KHỨ, BẢO TRÌ, TRÙNG LỊCH)
            // =========================================================
            if (request.NewSlots != null && request.NewSlots.Any())
            {
                // Convert từ ChangeRequestSlot sang BookingSlot để tái sử dụng hàm check
                var slotsToCheck = request.NewSlots.Select(s => new BookingSlot
                {
                    Date = s.Date,
                    SlotId = s.SlotId
                }).ToList();

                // Gọi hàm GetConflictingSlotsAsync (Hàm này bạn đã có ở trên)
                // excludeBookingId = booking.Id (để không tự báo trùng với chính mình)
                var conflicts = await GetConflictingSlotsAsync(booking.LabRoomId, slotsToCheck, booking.Id);

                if (conflicts.Any())
                {
                    var firstConflict = conflicts.First();
                    string errorMsg;

                    switch (firstConflict.Reason)
                    {
                        case UnavailableReason.PastTime:
                            errorMsg = $"Không thể duyệt: Khung giờ ngày {firstConflict.Date:dd/MM/yyyy} đã trôi qua.";
                            break;
                        case UnavailableReason.Maintenance:
                            errorMsg = $"Không thể duyệt: Khung giờ ngày {firstConflict.Date:dd/MM/yyyy} trùng lịch bảo trì.";
                            break;
                        default:
                            errorMsg = $"Không thể duyệt: Khung giờ ngày {firstConflict.Date:dd/MM/yyyy} đã bị trùng với đơn khác.";
                            break;
                    }

                    throw new BadRequestException(errorMsg);
                }
            }

            // =========================================================
            // 3. APPLY CHANGES (GHI ĐÈ DỮ LIỆU)
            // =========================================================

            // -- 3.1 Thông tin cơ bản --
            if (!string.IsNullOrEmpty(request.NewTitle)) booking.Title = request.NewTitle;
            if (!string.IsNullOrEmpty(request.NewDescription)) booking.Description = request.NewDescription;
            if (request.NewNumberOfParticipants.HasValue) booking.NumberOfParticipants = request.NewNumberOfParticipants.Value;
            if (request.NewCourseId.HasValue) booking.CourseId = request.NewCourseId;

            // -- 3.2 Cập nhật Project (Có LoadAsync để tránh lỗi) --
            if (!string.IsNullOrEmpty(request.NewProjectJson))
            {
                // Load Project hiện tại lên RAM
                await dbContext.Entry(booking).Reference(b => b.Project).LoadAsync();

                var newProject = JsonSerializer.Deserialize<Project>(request.NewProjectJson);
                if (newProject != null)
                {
                    if (booking.Project != null)
                    {
                        // Update
                        booking.Project.ProjectName = newProject.ProjectName;
                        booking.Project.Description = newProject.Description;
                        booking.Project.ProjectType = newProject.ProjectType;
                    }
                    else
                    {
                        // Insert mới
                        newProject.Id = Guid.NewGuid(); // Đảm bảo ID mới
                                                        // newProject.BookingId = booking.Id; // Nếu cần gán FK thủ công
                        booking.Project = newProject; // Gán navigation property
                    }
                }
            }

            // -- 3.3 Cập nhật PriorityDetail (Có LoadAsync) --
            if (!string.IsNullOrEmpty(request.NewPriorityDetailJson))
            {
                await dbContext.Entry(booking).Reference(b => b.BookingPriorityDetail).LoadAsync();

                var newPriority = JsonSerializer.Deserialize<BookingPriorityDetail>(request.NewPriorityDetailJson);
                if (newPriority != null)
                {
                    if (booking.BookingPriorityDetail != null)
                    {
                        booking.BookingPriorityDetail.Justification = newPriority.Justification;
                        booking.BookingPriorityDetail.EvidenceFilePath = newPriority.EvidenceFilePath;
                    }
                    else
                    {
                        newPriority.Id = Guid.NewGuid();
                        booking.BookingPriorityDetail = newPriority;
                    }
                }
            }

            // -- 3.4 Cập nhật Thiết bị (External Equipments) --
            if (!string.IsNullOrEmpty(request.NewExternalEquipmentsJson))
            {
                // Xóa cũ
                var oldEquips = await dbContext.ExternalEquipments
                    .Where(e => e.BookingId == booking.Id)
                    .ToListAsync();
                dbContext.ExternalEquipments.RemoveRange(oldEquips);

                // Thêm mới
                var newEquips = JsonSerializer.Deserialize<List<ExternalEquipment>>(request.NewExternalEquipmentsJson);
                if (newEquips != null)
                {
                    foreach (var eq in newEquips)
                    {
                        eq.Id = Guid.NewGuid();
                        eq.BookingId = booking.Id;
                        dbContext.ExternalEquipments.Add(eq);
                    }
                }
            }

            // -- 3.5 Cập nhật Khách mời (OutSide Guests) --
            if (!string.IsNullOrEmpty(request.NewOutSideGuestsJson))
            {
                var oldGuests = await dbContext.OutSideGuests
                    .Where(g => g.BookingId == booking.Id)
                    .ToListAsync();
                dbContext.OutSideGuests.RemoveRange(oldGuests);

                var newGuests = JsonSerializer.Deserialize<List<OutSideGuest>>(request.NewOutSideGuestsJson);
                if (newGuests != null)
                {
                    foreach (var g in newGuests)
                    {
                        g.Id = Guid.NewGuid();
                        g.BookingId = booking.Id;
                        dbContext.OutSideGuests.Add(g);
                    }
                }
            }

            // -- 3.6 CẬP NHẬT SLOTS (LOGIC QUAN TRỌNG) --
            if (request.NewSlots != null && request.NewSlots.Any())
            {
                // Lấy danh sách slot đang Active hiện tại
                var currentActiveSlots = await dbContext.BookingSlots
                    .Where(s => s.BookingId == booking.Id && s.Status == BookingSlotStatus.Active)
                    .ToListAsync();

                // A. Hủy (Cancel) các slot cũ KHÔNG còn nằm trong danh sách mới
                foreach (var oldSlot in currentActiveSlots)
                {
                    var stillExists = request.NewSlots.Any(newS =>
                        newS.Date == oldSlot.Date && newS.SlotId == oldSlot.SlotId);

                    if (!stillExists)
                    {
                        oldSlot.Status = BookingSlotStatus.Cancelled;
                        dbContext.Entry(oldSlot).State = EntityState.Modified;
                    }
                }

                // B. Thêm các slot mới chưa có trong DB
                foreach (var newReqSlot in request.NewSlots)
                {
                    var alreadyExists = currentActiveSlots.Any(old =>
                        old.Date == newReqSlot.Date && old.SlotId == newReqSlot.SlotId);

                    if (!alreadyExists)
                    {
                        var newSlotEntity = new BookingSlot
                        {
                            Id = Guid.NewGuid(),
                            BookingId = booking.Id,
                            SlotId = newReqSlot.SlotId,
                            Date = newReqSlot.Date,
                            Status = BookingSlotStatus.Active,
                            Priority = (int)booking.Priority, // Giữ nguyên mức ưu tiên của Booking
                            Reason = UnavailableReason.Booked
                        };
                        dbContext.BookingSlots.Add(newSlotEntity);
                    }
                }
            }

            // 4. CẬP NHẬT TRẠNG THÁI REQUEST
            request.Status = BookingChangeRequestStatus.Approved;
            // request.ApprovedById = managerId; // Nếu có trường này
            dbContext.Entry(request).State = EntityState.Modified;

            // 5. LƯU THAY ĐỔI
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<BookingSlot>> GetConflictingSlotsAsync(Guid labRoomId, List<BookingSlot> requestedSlots, Guid? excludeBookingId = null)
        {
            try
            {
                // 1. Validate
                if (requestedSlots == null || !requestedSlots.Any()) return new List<BookingSlot>();

                var dates = requestedSlots.Select(s => s.Date).Distinct().ToList();
                var slotIds = requestedSlots.Select(s => s.SlotId).Distinct().ToList();

                // 2. Prepare Data for DB Query (Fix PostgreSQL UTC)
                var minDateRaw = dates.Min().ToDateTime(TimeOnly.MinValue);
                var minDate = DateTime.SpecifyKind(minDateRaw, DateTimeKind.Utc);

                var maxDateRaw = dates.Max().ToDateTime(TimeOnly.MaxValue);
                var maxDate = DateTime.SpecifyKind(maxDateRaw, DateTimeKind.Utc);

                // =========================================================
                // [MỚI] PHẦN 0: CHECK SLOT TRONG QUÁ KHỨ (PAST TIME)
                // =========================================================

                // A. Cần lấy Master Data của Slot NGAY TỪ ĐẦU để biết giờ bắt đầu
                var slotMasterData = await dbContext.Slots
                    .AsNoTracking()
                    .Where(s => slotIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id);

                // B. Lấy giờ hiện tại ở Việt Nam
                var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowInVietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);

                var pastConflicts = new List<BookingSlot>();

                foreach (var reqSlot in requestedSlots)
                {
                    if (!slotMasterData.TryGetValue(reqSlot.SlotId, out var masterSlot)) continue;

                    // Tính thời gian bắt đầu của slot: Ngày (từ request) + Giờ (từ DB)
                    var slotStartTime = reqSlot.Date.ToDateTime(masterSlot.StartTime);

                    // So sánh: Nếu thời gian bắt đầu < Thời gian hiện tại -> Lỗi
                    if (slotStartTime < nowInVietnam)
                    {
                        pastConflicts.Add(new BookingSlot
                        {
                            Date = reqSlot.Date,
                            SlotId = reqSlot.SlotId,
                            // Bạn nên thêm 1 giá trị enum mới như 'PastTime' hoặc dùng tạm 'Locked'
                            Reason = UnavailableReason.PastTime // Hoặc tạo UnavailableReason.PastTime
                        });
                    }
                }

                // Nếu có slot trong quá khứ -> Return ngay lập tức, khỏi check cái khác
                if (pastConflicts.Any()) return pastConflicts;


                // =========================================================
                // PHẦN 1: CHECK TRÙNG VỚI BOOKING KHÁC
                // =========================================================
                var bookedConflicts = await dbContext.BookingSlots
                    .AsNoTracking()
                    .Where(bs =>
                        bs.Booking.LabRoomId == labRoomId &&
                        bs.Status == BookingSlotStatus.Active &&
                        dates.Contains(bs.Date) &&
                        slotIds.Contains(bs.SlotId) &&
                        (excludeBookingId == null || bs.BookingId != excludeBookingId)
                    )
                    .ToListAsync();

                var realConflicts = bookedConflicts.Where(dbSlot =>
                    requestedSlots.Any(req => req.Date == dbSlot.Date && req.SlotId == dbSlot.SlotId)
                ).ToList();

                if (realConflicts.Any()) return realConflicts;


                // =========================================================
                // PHẦN 2: CHECK TRÙNG LỊCH BẢO TRÌ
                // =========================================================

                var maintenanceSchedules = await dbContext.RoomMaintainSchedules
                    .AsNoTracking()
                    .Where(m =>
                        m.LabRoomId == labRoomId &&
                        m.EndTime > minDate &&
                        m.StartTime < maxDate &&
                        m.RoomMaintainStatus == RoomMaintainStatus.NotYet
                    ).ToListAsync();

                if (!maintenanceSchedules.Any()) return new List<BookingSlot>();

                // (Đã lấy slotMasterData ở trên rồi, không cần query lại nữa)

                foreach (var reqSlot in requestedSlots)
                {
                    if (!slotMasterData.TryGetValue(reqSlot.SlotId, out var masterSlot)) continue;

                    var localStart = reqSlot.Date.ToDateTime(masterSlot.StartTime);
                    var localEnd = reqSlot.Date.ToDateTime(masterSlot.EndTime);

                    var utcSlotStart = TimeZoneInfo.ConvertTime(localStart, localTimeZone, TimeZoneInfo.Utc);
                    var utcSlotEnd = TimeZoneInfo.ConvertTime(localEnd, localTimeZone, TimeZoneInfo.Utc);

                    var isUnderMaintenance = maintenanceSchedules.Any(m =>
                        utcSlotStart < m.EndTime && utcSlotEnd > m.StartTime
                    );

                    if (isUnderMaintenance)
                    {
                        realConflicts.Add(new BookingSlot
                        {
                            Date = reqSlot.Date,
                            SlotId = reqSlot.SlotId,
                            Reason = UnavailableReason.Maintenance
                        });
                    }
                }

                return realConflicts;
            }
            catch (Exception ex)
            {
                // Log error here
                throw new Exception("Có lỗi xảy ra.");
            }
        }
    }
}
