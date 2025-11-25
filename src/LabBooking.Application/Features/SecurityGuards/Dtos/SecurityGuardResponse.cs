namespace LabBooking.Application.Features.SecurityGuards.Dtos;

public record SecurityGuardResponse(
    Guid Id,
    string Email,
    string UserName,
    string? PhoneNumber
// Thêm FullName nếu entity User của bạn có
);
