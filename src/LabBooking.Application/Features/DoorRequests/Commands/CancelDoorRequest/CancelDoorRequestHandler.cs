using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.DoorRequests.Commands.CancelDoorRequest;

public class CancelDoorRequestHandler(
    IDoorRequestRepository repo,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CancelDoorRequestCommand, bool>
{
    public async Task<bool> Handle(CancelDoorRequestCommand command, CancellationToken cancellationToken)
    {
        // 1. Lấy ID người đang thao tác
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        // 2. Lấy yêu cầu từ DB
        var request = await repo.GetByIdAsync(command.RequestId, cancellationToken);

        if (request == null)
            throw new NotFoundException(nameof(DoorOpeningRequest), command.RequestId.ToString());

        // --- RULE 1: CHECK CHÍNH CHỦ ---
        // Nếu không phải là người tạo ra yêu cầu này -> Cấm
        if (request.RequestedById != currentUserId)
        {
            throw new ForbidException("Bạn không có quyền hủy yêu cầu của người khác.");
        }

        // --- RULE 2: CHECK TRẠNG THÁI ---
        // Chỉ được hủy khi đang PENDING (Chưa ai nhận)
        if (request.Status != DoorRequestStatus.Pending)
        {
            // Tùy chỉnh thông báo lỗi cho thân thiện
            if (request.Status == DoorRequestStatus.Accepted)
                throw new BadRequestException("Bảo vệ đã tiếp nhận và đang đến, không thể hủy lúc này!");

            if (request.Status == DoorRequestStatus.Completed)
                throw new BadRequestException("Yêu cầu đã hoàn thành, không thể hủy.");

            throw new BadRequestException("Không thể hủy yêu cầu này.");
        }

        // 3. Cập nhật trạng thái -> Cancelled
        request.Status = DoorRequestStatus.Cancelled;

        // (Optional) Xóa luôn khỏi DB nếu bạn không muốn lưu rác
        // Nhưng tốt nhất là Update Status để lưu lịch sử là "User đã từng hủy"
        await repo.UpdateAsync(request, cancellationToken);

        return true;
    }
}
