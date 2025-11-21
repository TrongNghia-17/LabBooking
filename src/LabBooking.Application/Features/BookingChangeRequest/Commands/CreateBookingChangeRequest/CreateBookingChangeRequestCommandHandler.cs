using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest
{
    public class CreateBookingChangeRequestCommandHandler(
    IBookingChangeRequestRepository changeRequestRepository
    ) : IRequestHandler<CreateBookingChangeRequestCommand, Guid>
    {
        public async Task<Guid> Handle(CreateBookingChangeRequestCommand request, CancellationToken token)
        {
            // =================================================================
            // BƯỚC 1: CHECK TỒN TẠI & QUYỀN (GỌI REPO)
            // =================================================================

            var isValid = await changeRequestRepository.IsBookingOwnerAndApprovedAsync(
                request.BookingId,
                request.RequestedById
            );

            if (!isValid)
            {
                // Nếu false thì có thể do:
                // 1. Không tìm thấy Booking
                // 2. Không phải chính chủ
                // 3. Booking chưa được Approved

                // Ném lỗi NotFound (hoặc BadRequestException tùy bạn)
                throw new NotFoundException(nameof(Booking), request.BookingId.ToString());
            }

            // =================================================================
            // BƯỚC 2: TẠO CHANGE REQUEST ENTITY
            // =================================================================

            var changeRequest = new Domain.Entities.BookingChangeRequest
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                RequestedById = request.RequestedById,

                // --- 2.1 Map các trường thông tin cơ bản ---
                NewTitle = request.NewTitle,
                NewDescription = request.NewDescription,
                NewNumberOfParticipants = request.NewNumberOfParticipants,
                NewCourseId = request.NewCourseId,

                // --- 2.2 Map Object phức tạp -> JSON String (Serialize) ---

                // Lưu thông tin Project (nếu có thay đổi hoặc giữ nguyên)
                NewProjectJson = request.NewProject != null
                    ? JsonSerializer.Serialize(request.NewProject)
                    : null,

                // Lưu thông tin Priority Detail
                NewPriorityDetailJson = request.NewPriorityDetail != null
                    ? JsonSerializer.Serialize(request.NewPriorityDetail)
                    : null,

                // Lưu danh sách thiết bị (List -> JSON Array)
                NewExternalEquipmentsJson = request.NewExternalEquipments != null
                    ? JsonSerializer.Serialize(request.NewExternalEquipments)
                    : null,

                // --- 2.3 Set trạng thái khởi tạo ---
                Status = BookingChangeRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // =================================================================
            // BƯỚC 3: XỬ LÝ SLOTS (SNAPSHOT)
            // =================================================================

            // Lưu toàn bộ danh sách slot mà user mong muốn sở hữu trong tương lai
            // (Bao gồm cả slot cũ giữ lại + slot mới thêm vào)
            if (request.DesiredSlots != null && request.DesiredSlots.Any())
            {
                changeRequest.NewSlots = request.DesiredSlots.Select(s => new BookingChangeRequestSlot
                {
                    Id = Guid.NewGuid(),
                    // BookingChangeRequestId sẽ được EF Core tự động gán khi lưu object cha
                    SlotId = s.SlotId,
                    Date = s.Date
                }).ToList();
            }

            // =================================================================
            // BƯỚC 4: LƯU VÀO DATABASE
            // =================================================================

            var createdRequest = await changeRequestRepository.CreateAsync(changeRequest);

            return createdRequest.Id;
        }
    }
}
