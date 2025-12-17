using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.DoorRequests.Commands.DeleteDoorRequest;

public class DeleteDoorRequestHandler(
    IDoorRequestRepository doorRequestRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<DeleteDoorRequestCommand>
{
    public async Task Handle(DeleteDoorRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId == Guid.Empty) throw new UnauthorizedAccessException();

        // 1. Tìm bản ghi
        var entity = await doorRequestRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());

        // 2. Check quyền sở hữu (Chỉ chủ nhân mới được xóa)
        if (entity.RequestedById != currentUserId)
        {
            // Báo lỗi 403 Forbidden
            throw new ForbidException("Bạn không có quyền xóa yêu cầu của người khác.");
        }

        // 3. Check trạng thái (Chỉ xóa được khi chưa ai đụng vào)
        if (entity.Status != DoorRequestStatus.Pending)
        {
            // Báo lỗi 400 Bad Request
            throw new BadRequestException("Không thể xóa yêu cầu đã được xử lý (Duyệt/Từ chối).");
        }

        // 4. Thực hiện xóa
        await doorRequestRepository.DeleteAsync(entity);
    }
}