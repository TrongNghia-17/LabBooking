using LabBooking.Application.Features.RoomChecks.Dtos;

namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public record CreateRoomCheckCommand(
    Guid LabRoomId,
    CheckType Type,
    string? Note,
    List<RoomCheckItemDto> EquipmentDetails
) : IRequest<Guid>;
