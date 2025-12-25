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
        // 1. Validate User & Lab
        var guardId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var labExists = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Lab ID {request.LabRoomId} not found.");

        // 2. [FIX LOGIC] CHỐNG SPAM THEO TYPE
        if (request.SlotId.HasValue)
        {
            // Kiểm tra: Phòng + Slot + Ngày + LOẠI CHECK (In/Out)
            bool isChecked = await roomCheckRepository.ExistsAsync(
                request.LabRoomId,
                request.SlotId.Value,
                request.Type,
                DateTime.UtcNow,
                cancellationToken);

            if (isChecked)
            {
                throw new BadRequestException($"Phòng này đã được thực hiện '{request.Type}' trong Slot này rồi. Vui lòng xóa phiếu cũ nếu muốn tạo lại.");
            }
        }

        // 3. Map Entity
        var roomCheck = mapper.Map<RoomCheck>(request);
        roomCheck.GuardId = guardId;
        roomCheck.CheckedAt = DateTime.UtcNow;

        // 4. Save
        await roomCheckRepository.AddAsync(roomCheck, cancellationToken);

        return roomCheck.Id;
    }
}