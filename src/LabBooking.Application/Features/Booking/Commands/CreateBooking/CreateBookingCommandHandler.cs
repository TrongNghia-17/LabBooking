

using LabBooking.Application.Features.Booking.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LabBooking.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(
    ILogger<CreateBookingCommandHandler> logger,
    IMapper mapper,
    IBookingRepository bookingRepository,
    IProjectRepository projectRepository
    ) : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new booking of type {Type}", request.Type);

        // 1. Map thông tin cơ bản
        var booking = new Domain.Entities.Booking
        {
            Id = Guid.NewGuid(), // Tạo ID mới luôn
            LabRoomId = request.LabRoomId,
            CreatedById = request.CreatedById,
            Title = request.Title,
            Description = request.Description,
            NumberOfParticipants = request.NumberOfParticipants,
            IsPublic = request.IsPublic,
            IsMajorOnly = request.IsMajorOnly,
            Type = request.Type,
            Status = BookingStatus.Pending
        };

        // 2. Xử lý logic riêng theo từng loại (Gán Priority & Data đi kèm)
        switch (request.Type)
        {
            case BookingType.Teaching:
                // Dạy học: Priority 2, Cần CourseId
                booking.Priority = BookingPriority.Standard;
                booking.CourseId = request.CourseId;
                break;

            case BookingType.Project:
                // Dự án: Priority 2, Tạo mới Project nếu có
                booking.Priority = BookingPriority.Standard;

                if (request.Project != null)
                {
                    var newProject = new Project
                    {
                        Id = Guid.NewGuid(),
                        ProjectName = request.Project.ProjectName,
                        Description = request.Project.Description,
                        ProjectType = request.Project.ProjectType,
                        OwnerId = request.CreatedById
                    };

                    // Lưu Project trước
                    await projectRepository.CreateProjectAsync(newProject);
                    booking.ProjectId = newProject.Id;
                }
                break;

            case BookingType.UniversityEvent:
                // Sự kiện trường: Priority 1 (High/VIP)
                booking.Priority = BookingPriority.UniversityEvent; // Giá trị là 1

                if (request.PriorityDetail != null)
                {
                    var priorityDetail = new Domain.Entities.BookingPriorityDetail
                    {
                        Id = Guid.NewGuid(),
                        Justification = request.PriorityDetail.Justification,
                        EvidenceFilePath = request.PriorityDetail.EvidenceFilePath
                        // BookingId sẽ được gán tự động bởi EF Core
                    };

                    // Gán vào Navigation Property
                    // Lưu ý: Trong Entity Booking của bạn tên là 'BookingPriorityDetail' hay 'PriorityDetail'?
                    // Dựa trên model bạn gửi: "public BookingPriorityDetail? BookingPriorityDetail { get; set; }"
                    booking.BookingPriorityDetail = priorityDetail;
                    booking.BookingPriorityDetailId = priorityDetail.Id;
                }
                break;

            default:
                booking.Priority = BookingPriority.Standard;
                break;
        }

        if (request.ExternalEquipments != null && request.ExternalEquipments.Any())
        {
            booking.ExternalEquipments = request.ExternalEquipments.Select(e => new Domain.Entities.ExternalEquipment
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id, // Gán ID cha
                EquipmentName = e.Name,
                Description = e.Description,
                Quantity = e.Quantity
            }).ToList();
        }

        // 3. Xử lý Slots
        if (request.Slots != null && request.Slots.Any())
        {
            booking.Slots = request.Slots.Select(s => new BookingSlot
            {
                Id = Guid.NewGuid(),
                Date = s.Date,
                SlotId = s.SlotId,
                // Gán Priority cho Slot (ép kiểu Enum sang int để lưu vào DB)
                Priority = (int)booking.Priority,
                Reason = UnavailableReason.Booked
            }).ToList();
        }

        // ==========================================================
        // 4. [MỚI] Xử lý Khách mời bên ngoài (OutSide Guests)
        // ==========================================================
        if (request.OutSideGuests != null && request.OutSideGuests.Any())
        {
            // Logic: Lấy ngày slot đầu tiên làm ngày tham quan mặc định
            // Vì Input OutSideGuest không có ngày, mà Entity bắt buộc có VisitDate
            var defaultVisitDate = request.Slots.Any()
                ? request.Slots.Min(s => s.Date).ToDateTime(new TimeOnly(0, 0))
                : DateTime.UtcNow;

            booking.OutSideGuests = request.OutSideGuests.Select(g => new Domain.Entities.OutSideGuest
            {
                Id = Guid.NewGuid(),
                // Giả sử Entity OutSideGuest của bạn có BookingId để link
                // Nếu chưa có, bạn cần thêm prop public Guid BookingId { get; set; } vào Entity OutSideGuest
                // BookingId = booking.Id, // EF Core sẽ tự gán nếu add vào collection của booking

                FullName = g.FullName,
                Email = g.Email,
                Organization = g.Organization,
                PurposeOfVisit = g.purpose,
                VisitDate = defaultVisitDate, // Gán ngày

                CreatedById = request.CreatedById,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            }).ToList();
        }

        // 4. Gọi Repository để lưu (bao gồm cả check conflict/override trong Repository)
        var createdBooking = await bookingRepository.CreateBookingAsync(booking);


        // 5. Map ra response
        // (Đảm bảo MappingProfile đã cấu hình đúng)
        var any = mapper.Map<BookingResponse>(createdBooking);
        return any;
    }
}