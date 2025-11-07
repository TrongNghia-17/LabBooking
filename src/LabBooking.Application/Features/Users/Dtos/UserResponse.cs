namespace LabBooking.Application.Features.Users.Dtos;

/// <summary>
/// DTO trả về thông tin cơ bản của người dùng.
/// </summary>
public record UserResponse(
    Guid Id,
    string? UserName,
    string? Email,
    string? PhoneNumber,
    string? Major,
    DateTime RegistrationDate
);
