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
                throw new InvalidOperationException("Không tìm thấy yêu cầu thay đổi.");

            if (request.Status != BookingChangeRequestStatus.Pending)
                throw new InvalidOperationException("Yêu cầu này không còn ở trạng thái chờ.");

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
            // 1. Lấy Request
            var request = await GetRequestWithDetailsAsync(requestId);
            if (request == null) throw new InvalidOperationException("Change Request not found");
            if (request.Status != BookingChangeRequestStatus.Pending)
                throw new InvalidOperationException("Request is not pending");

            var booking = request.Booking; // Booking gốc

            // 2. CHECK CONFLICT CHO SLOTS MỚI
            // (Logic này nên tái sử dụng từ BookingRepository nếu được, ở đây mình viết lại cho rõ)
            if (request.NewSlots != null && request.NewSlots.Any())
            {
                // Convert SlotEntity của ChangeRequest sang dạng đơn giản để check
                var reqDates = request.NewSlots.Select(s => s.Date).Distinct().ToList();
                var reqSlotIds = request.NewSlots.Select(s => s.SlotId).Distinct().ToList();

                // Tìm các slot ĐANG ACTIVE của người khác bị trùng
                var conflicts = await dbContext.BookingSlots
                    .Include(bs => bs.Booking)
                    .Where(bs =>
                        bs.Booking.LabRoomId == booking.LabRoomId && // Cùng phòng
                        reqDates.Contains(bs.Date) &&
                        reqSlotIds.Contains(bs.SlotId) &&
                        bs.Status == BookingSlotStatus.Active &&
                        bs.BookingId != booking.Id // Không tính chính nó (quan trọng!)
                    )
                    .ToListAsync();

                // Lọc chính xác
                var realConflicts = conflicts.Where(dbSlot =>
                    request.NewSlots.Any(req => req.Date == dbSlot.Date && req.SlotId == dbSlot.SlotId)
                ).ToList();

                if (realConflicts.Any())
                {
                    // Nếu trùng -> Chặn luôn (hoặc xử lý Override nếu là Priority cao - tùy nghiệp vụ)
                    throw new InvalidOperationException("Xung đột lịch với đơn khác. Không thể duyệt.");
                }
            }

            // 3. APPLY CHANGES (GHI ĐÈ DỮ LIỆU TỪ REQUEST SANG BOOKING)
            //using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // 3.1 Cập nhật thông tin cơ bản
                if (!string.IsNullOrEmpty(request.NewTitle)) booking.Title = request.NewTitle;
                if (!string.IsNullOrEmpty(request.NewDescription)) booking.Description = request.NewDescription;
                if (request.NewNumberOfParticipants.HasValue) booking.NumberOfParticipants = request.NewNumberOfParticipants.Value;
                if (request.NewCourseId.HasValue) booking.CourseId = request.NewCourseId;

                // 3.2 Cập nhật Object JSON (Project, Priority...)
                // Lưu ý: Logic này tùy thuộc vào việc bạn lưu JSON hay tách bảng. 
                // Nếu tách bảng (như Project, PriorityDetail) thì phải update bảng con.

                // Ví dụ với Project (Nếu tách bảng):
                if (!string.IsNullOrEmpty(request.NewProjectJson))
                {
                    var newProject = JsonSerializer.Deserialize<Project>(request.NewProjectJson);
                    if (booking.Project != null)
                    {
                        // Update cái cũ
                        booking.Project.ProjectName = newProject.ProjectName;
                        booking.Project.Description = newProject.Description;
                        booking.Project.ProjectType = newProject.ProjectType;
                    }
                    else
                    {
                        // Tạo mới nếu chưa có
                        //newProject = booking.Id;
                        dbContext.Projects.Add(newProject);
                    }
                }

                // Tương tự cho PriorityDetail...
                if (!string.IsNullOrEmpty(request.NewPriorityDetailJson))
                {
                    var newPriorityDetail = JsonSerializer.Deserialize<BookingPriorityDetail>(request.NewPriorityDetailJson);
                    if (booking.BookingPriorityDetail != null)
                    {
                        // Update cái cũ
                        booking.BookingPriorityDetail.Justification = newPriorityDetail.Justification;
                        booking.BookingPriorityDetail.EvidenceFilePath = newPriorityDetail.EvidenceFilePath;
                    }
                    else
                    {
                        // Tạo mới nếu chưa có
                        //newPriorityDetail.BookingId = booking.Id;
                        dbContext.BookingPriorityDetails.Add(newPriorityDetail);
                    }
                }

                

                // 3.3 Cập nhật Thiết bị (External Equipments)
                if (!string.IsNullOrEmpty(request.NewExternalEquipmentsJson))
                {
                    // Xóa thiết bị cũ
                    var oldEquips = dbContext.ExternalEquipments.Where(e => e.BookingId == booking.Id);
                    dbContext.ExternalEquipments.RemoveRange(oldEquips);

                    // Thêm thiết bị mới
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

                // [MỚI] Cập nhật Khách mời (Outside Guests)
                if (!string.IsNullOrEmpty(request.NewOutSideGuestsJson)) // Giả sử bạn đã thêm trường này vào Entity
                {
                    var oldGuests = dbContext.OutSideGuests.Where(g => g.BookingId == booking.Id);
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

                // 3.4 CẬP NHẬT SLOTS (PHẦN QUAN TRỌNG NHẤT)
                if (request.NewSlots != null && request.NewSlots.Any())
                {
                    // A. Hủy các slot cũ không còn nằm trong danh sách mới
                    var currentActiveSlots = await dbContext.BookingSlots
                        .Where(s => s.BookingId == booking.Id && s.Status == BookingSlotStatus.Active)
                        .ToListAsync();

                    foreach (var oldSlot in currentActiveSlots)
                    {
                        // Kiểm tra xem slot cũ này có tồn tại trong danh sách mong muốn mới không
                        var stillExists = request.NewSlots.Any(newS =>
                            newS.Date == oldSlot.Date && newS.SlotId == oldSlot.SlotId);

                        if (!stillExists)
                        {
                            // Nếu không còn -> Đánh dấu Cancelled (hoặc xóa hẳn tùy bạn)
                            oldSlot.Status = BookingSlotStatus.Cancelled;
                        }
                    }

                    // B. Thêm các slot mới chưa có trong DB
                    foreach (var newReqSlot in request.NewSlots)
                    {
                        var alreadyExists = currentActiveSlots.Any(old =>
                            old.Date == newReqSlot.Date && old.SlotId == newReqSlot.SlotId);

                        if (!alreadyExists)
                        {
                            // Tạo slot mới
                            var newSlotEntity = new BookingSlot
                            {
                                Id = Guid.NewGuid(),
                                BookingId = booking.Id,
                                SlotId = newReqSlot.SlotId,
                                Date = newReqSlot.Date,
                                Status = BookingSlotStatus.Active,
                                Priority = (int)booking.Priority,
                                Reason = UnavailableReason.Booked
                            };
                            dbContext.BookingSlots.Add(newSlotEntity);
                        }
                    }
                }

                // 4. Update trạng thái Request -> Approved
                request.Status = BookingChangeRequestStatus.Approved;
                // request.ManagerId = managerId; // Lưu ai duyệt

                //await dbContext.SaveChangesAsync();
                //await transaction.CommitAsync();
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
