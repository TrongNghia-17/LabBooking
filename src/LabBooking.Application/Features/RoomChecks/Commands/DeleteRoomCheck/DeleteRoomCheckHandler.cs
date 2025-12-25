using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomChecks.Commands.DeleteRoomCheck;

public class DeleteRoomCheckHandler(
    IRoomCheckRepository roomCheckRepository,
    IIncidentRepository incidentRepository, // THÊM LẠI: Cần repository này để check ràng buộc
    ICurrentUserService currentUserService
    ) : IRequestHandler<DeleteRoomCheckCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomCheckCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện hành động này.");

        // 1. Tìm RoomCheck trong database
        var roomCheck = await roomCheckRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(RoomCheck), request.Id.ToString());

        // --- BẮT ĐẦU CHUỖI KIỂM TRA ĐIỀU KIỆN ---

        // 2. [ĐIỀU KIỆN 1] Kiểm tra quyền sở hữu
        if (roomCheck.GuardId != currentUserId)
        {
            throw new ForbiddenAccessException("Chỉ người tạo phiếu kiểm tra mới có quyền xóa.");
        }

        // 3. [ĐIỀU KIỆN 2] Kiểm tra giới hạn thời gian 24 giờ
        var timeElapsed = DateTime.UtcNow - roomCheck.CheckedAt;
        if (timeElapsed.TotalHours > 24)
        {
            throw new BadRequestException("Không thể xóa phiếu kiểm tra đã được tạo quá 24 giờ.");
        }

        // 4. [ĐIỀU KIỆN 3] Kiểm tra ràng buộc với Incident (Logic cũ)
        bool hasIncident = await incidentRepository.HasActiveIncidentForRoomCheckAsync(request.Id, cancellationToken);
        if (hasIncident)
        {
            throw new BadRequestException(
                "Không thể xóa phiếu kiểm tra này vì đang có Sự cố (Incident) liên quan. " +
                "Vui lòng xóa Sự cố trước khi xóa phiếu kiểm tra.");
        }

        // 5. Nếu vượt qua tất cả các điều kiện -> Thực hiện xóa mềm
        await roomCheckRepository.SoftDeleteAsync(roomCheck, cancellationToken);

        return Unit.Value;
    }
}