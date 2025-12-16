using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.DoorRequests.Commands.CancelDoorRequest;

public class CancelDoorRequestHandler(
    IDoorRequestRepository repo,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CancelDoorRequestCommand, bool>
{
    public async Task<bool> Handle(CancelDoorRequestCommand command, CancellationToken cancellationToken)
    {
        // 1. Lấy ID người dùng hiện tại
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        // 2. Lấy yêu cầu từ DB
        var request = await repo.GetByIdAsync(command.RequestId, cancellationToken);

        if (request == null)
            throw new NotFoundException(nameof(DoorOpeningRequest), command.RequestId.ToString());

        // --- VALIDATION 1: CHÍNH CHỦ ---
        if (request.RequestedById != currentUserId)
        {
            throw new ForbidException("Bạn không có quyền xóa yêu cầu của người khác.");
        }

        // --- VALIDATION 2: TRẠNG THÁI (Logic bạn yêu cầu) ---
        // Chỉ cho phép xóa khi trạng thái là PENDING (Chưa ai nhận)
        if (request.Status != DoorRequestStatus.Pending)
        {
            // Trường hợp 1: Đã được bảo vệ chấp nhận
            if (request.Status == DoorRequestStatus.Accepted)
            {
                throw new BadRequestException("Yêu cầu đã được Bảo vệ chấp nhận và đang xử lý. Không thể xóa lúc này!");
            }

            // Trường hợp 2: Đã hoàn thành hoặc các trạng thái khác
            if (request.Status == DoorRequestStatus.Completed)
            {
                throw new BadRequestException("Yêu cầu đã hoàn thành xong, không thể xóa (nên giữ lại làm lịch sử).");
            }

            // Chặn tất cả các trường hợp còn lại
            throw new BadRequestException($"Không thể xóa yêu cầu đang ở trạng thái {request.Status}.");
        }

        // 3. THỰC HIỆN XÓA (HARD DELETE)
        // Nếu code chạy xuống được đến đây nghĩa là Status == Pending
        await repo.DeleteAsync(request, cancellationToken);

        return true;
    }
}