namespace LabBooking.Application.Features.Managers.Dtos;

public record ManagerProfileResponse(
    Guid Id,
    string UserName,
    string Email,
    List<ManagedLabRoomDto> ManagedLabs
);
public record ManagedLabRoomDto(
    Guid Id,
    string LabName,
    string Location
);

