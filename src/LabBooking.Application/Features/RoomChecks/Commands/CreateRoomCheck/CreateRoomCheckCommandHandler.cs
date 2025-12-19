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

        var labExists = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);
        if (labExists == null) throw new KeyNotFoundException($"Lab ID {request.LabRoomId} not found.");

        // 2. [QUAN TRỌNG] CHỐNG SPAM: Kiểm tra xem đã check chưa
        if (request.SlotId.HasValue)
        {
            // Kiểm tra xem Phòng này + Slot này + Hôm nay đã có phiếu nào chưa?
            bool isChecked = await roomCheckRepository.ExistsAsync(
                request.LabRoomId,
                request.SlotId.Value,
                DateTime.UtcNow, // Ngày hiện tại (UTC)
                cancellationToken);

            if (isChecked)
            {
                throw new BadRequestException("Phòng này đã được kiểm tra trong Slot này rồi. Vui lòng xóa phiếu cũ nếu muốn tạo lại.");
            }
        }

        // 3. Map Entity
        var roomCheck = mapper.Map<RoomCheck>(request);
        roomCheck.GuardId = guardId;
        // Đảm bảo gán ngày giờ server để đồng bộ với lúc check trùng
        roomCheck.CheckedAt = DateTime.UtcNow;

        // 4. Save
        await roomCheckRepository.AddAsync(roomCheck, cancellationToken);

        return roomCheck.Id;
    }
}