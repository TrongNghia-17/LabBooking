using MediatR;
using LabBooking.Domain.Entities;
using LabBooking.Application.Features.Booking.Dtos;

namespace LabBooking.Application.Features.Bookings.Commands.CreateBooking;

// 1. DTO cho Slot (Input)
public record BookingSlotInput(
    DateOnly Date,
    Guid SlotId
);

// 2. DTO cho Project (Nếu tạo mới)
public record CreateProjectInput(
    string ProjectName,
    string Description,
    ProjectType ProjectType
);

// 3. DTO cho Priority Detail (Nếu là sự kiện trường)
public record CreatePriorityDetailInput(
    string Justification,
    string? EvidenceFilePath
);

public record ExternalEquipmentInput(
    string Name,
    string? Description,
    int Quantity
);

// 4. COMMAND CHÍNH
public record CreateBookingCommand(
    Guid LabRoomId,
    Guid CreatedById,
    string Title,
    string? Description,
    int NumberOfParticipants,

    // Cờ cấu hình
    bool IsPublic,
    bool IsMajorOnly,

    // Loại đặt lịch (Quyết định logic xử lý)
    BookingType Type,

    // Danh sách Slot muốn đặt
    List<BookingSlotInput> Slots,

    // --- Các trường tùy chọn (Nullable) ---

    // Dành cho Teaching
    Guid? CourseId,

    // Dành cho Project
    CreateProjectInput? Project,

    // Dành cho UniversityEvent (Priority)
    CreatePriorityDetailInput? PriorityDetail,

    List<ExternalEquipmentInput>? ExternalEquipments

) : IRequest<BookingResponse>; // Giả sử bạn đã có BookingResponse