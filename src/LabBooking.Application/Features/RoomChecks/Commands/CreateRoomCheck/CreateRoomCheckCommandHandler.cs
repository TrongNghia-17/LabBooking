using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public class CreateRoomCheckCommandHandler(
    IRoomCheckRepository roomCheckRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<CreateRoomCheckCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoomCheckCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate logic
        var guardId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var labExists = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);
        if (labExists == null) throw new KeyNotFoundException($"Lab ID {request.LabRoomId} not found.");

        // 2. Map (Dùng AutoMapper cho gọn)
        var roomCheck = mapper.Map<RoomCheck>(request);
        roomCheck.GuardId = guardId;

        // 3. Save
        await roomCheckRepository.AddAsync(roomCheck, cancellationToken);

        return roomCheck.Id;
    }
}