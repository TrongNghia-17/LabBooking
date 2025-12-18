using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomChecks.Commands.DeleteRoomCheck;

public class DeleteRoomCheckHandler(
    IRoomCheckRepository roomCheckRepository,
    IIncidentRepository incidentRepository, // Cần cái này để check ràng buộc
    ICurrentUserService currentUserService
    ) : IRequestHandler<DeleteRoomCheckCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomCheckCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException();

        // 1. Tìm RoomCheck
        var roomCheck = await roomCheckRepository.GetByIdAsync(request.Id, cancellationToken);
        if (roomCheck == null) throw new NotFoundException(nameof(RoomCheck), request.Id.ToString());

        // 2. Check quyền (Chỉ người tạo hoặc Admin mới được xóa)
        // (Tùy logic đồ án của bạn, ở đây giả sử chỉ người tạo được xóa)
        if (roomCheck.GuardId != currentUserId)
        {
            // Nếu muốn Manager cũng xóa được thì check thêm ở đây
            throw new ForbiddenAccessException("Bạn không phải người tạo phiếu này.");
        }

        // 3. CHECK RÀNG BUỘC (QUAN TRỌNG)
        // Nếu phiếu check này đã đẻ ra Incident -> Không được xóa
        bool hasIncident = await incidentRepository.HasActiveIncidentForRoomCheckAsync(request.Id, cancellationToken);

        if (hasIncident)
        {
            throw new BadRequestException(
                "Không thể xóa phiếu kiểm tra này vì đang có Sự cố (Incident) liên quan. " +
                "Vui lòng xóa Sự cố trước khi xóa phiếu kiểm tra.");
        }

        // 4. Xóa mềm
        await roomCheckRepository.SoftDeleteAsync(roomCheck, cancellationToken);

        return Unit.Value;
    }
}