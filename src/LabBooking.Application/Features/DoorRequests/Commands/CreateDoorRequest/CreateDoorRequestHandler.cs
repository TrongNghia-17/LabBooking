using LabBooking.Application.Features.DoorRequests.Commands.Create;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public class CreateDoorRequestHandler(
    IDoorRequestRepository doorRequestRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<CreateDoorRequestHandler> logger
    ) : IRequestHandler<CreateDoorRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateDoorRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        // 2. Check Phòng tồn tại & Trạng thái cửa (Optional)
        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());

        // 3. Check Spam (Chống bấm liên tục)
        bool hasPending = await doorRequestRepository.HasPendingRequestAsync(currentUserId, request.LabRoomId, cancellationToken);
        if (hasPending)
        {
            throw new BadRequestException("Yêu cầu của bạn đang chờ bảo vệ xử lý.");
        }

        // 4. Tạo Request (Không cần check Booking - Tin tưởng người dùng)
        var newRequest = mapper.Map<DoorOpeningRequest>(request);

        // Gán nốt thông tin User (Vì thông tin này lấy từ Token, không có trong Command)
        newRequest.RequestedById = currentUserId;

        await doorRequestRepository.CreateAsync(newRequest, cancellationToken);

        logger.LogInformation("User {User} yêu cầu mở cửa phòng {Lab}", currentUserId, request.LabRoomId);

        return newRequest.Id;
    }
}