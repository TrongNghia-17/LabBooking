using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.DoorRequests.Commands.AcceptDoorRequest;

public class AcceptDoorRequestHandler(
    IDoorRequestRepository repo,
    ICurrentUserService currentUserService
    ) : IRequestHandler<AcceptDoorRequestCommand, bool>
{
    public async Task<bool> Handle(AcceptDoorRequestCommand command, CancellationToken cancellationToken)
    {
        // 1. Lấy ID bảo vệ đang thao tác
        var guardId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException();

        // 2. Lấy thông tin yêu cầu từ DB
        var doorRequest = await repo.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(DoorOpeningRequest), command.RequestId.ToString());

        // 3. --- CHỐT CHẶN QUAN TRỌNG (RACE CONDITION CHECK) ---
        // Nếu trạng thái KHÔNG CÒN LÀ PENDING (tức là đã có ai đó Accept hoặc Cancel rồi)
        if (doorRequest.Status != DoorRequestStatus.Pending)
        {
            // Báo lỗi ngay cho người đến sau
            throw new BadRequestException("Yêu cầu này đã được bảo vệ khác tiếp nhận!");
        }

        // 4. Nếu vẫn còn Pending -> Chốt đơn cho ông bảo vệ này
        doorRequest.Status = DoorRequestStatus.Accepted; // Chuyển trạng thái
        doorRequest.HandledById = guardId;               // Gán người nhận
        doorRequest.AcceptedTime = DateTime.UtcNow;   // (Nếu có trường này)

        // 5. Lưu xuống DB
        await repo.UpdateAsync(doorRequest, cancellationToken);

        return true;
    }
}
