using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public class CreateDoorRequestHandler(
    IDoorRequestRepository doorRequestRepository,
    IBookingRepository bookingRepository,
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository,
    IMapper mapper,
    ILogger<CreateDoorRequestHandler> logger
    ) : IRequestHandler<CreateDoorRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateDoorRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        // 1. Validate User
        if (currentUserId == Guid.Empty) throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        // 2. Validate quyền sở hữu Booking
        var isOwner = await bookingRepository.IsBookingOwnedByUserAsync(request.BookingCode, currentUserId!.Value);
        if (!isOwner)
        {
            throw new BadRequestException("Mã đặt phòng không hợp lệ hoặc bạn không có quyền.");
        }

        // 3. Lấy thông tin Manager từ Booking -> LabRoom -> MainManagerId
        var (Exists, ManagerId) = await bookingRepository.GetBookingAndManagerInfoAsync(request.BookingCode);
        if (!Exists)
        {
            throw new NotFoundException(nameof(Booking), request.BookingCode);
        }

        // 4. Map và gán dữ liệu
        var entity = mapper.Map<DoorOpeningRequest>(request);
        entity.RequestedById = currentUserId!.Value;

        // Gán Manager tìm được vào Request
        entity.ManagerId = ManagerId;

        // Log nếu phòng không có Manager
        if (entity.ManagerId == null)
        {
            logger.LogWarning($"Booking {request.BookingCode} thuộc phòng Lab chưa có MainManager.");
            throw new BadRequestException("Phòng Lab này chưa có quản lý để duyệt yêu cầu.");
        }

        // 5. Lưu yêu cầu vào DB
        var requestId = await doorRequestRepository.AddAsync(entity);

        // 6. GỬI THÔNG BÁO CHO MANAGER
        var pushQueue = new List<PushNotificationData>();

        var (notiEntity, pushData) = notificationRepository.PrepareNotification(
            entity.ManagerId.Value, // Manager ID
            "🔔 Yêu cầu mở cửa mới",
            $"Có yêu cầu mở cửa mới cho mã đặt phòng {request.BookingCode}. Vui lòng kiểm tra và duyệt.",
            "DOOR_REQUEST_PENDING",
            new
            {
                requestId = requestId,
                bookingCode = request.BookingCode,
                reason = request.Reason,
                requestedById = currentUserId.Value
            }
        );

        pushQueue.Add(pushData);

        await notificationRepository.CreateAsync(notiEntity, cancellationToken);

        // Chạy background task gửi push notification
        notificationRepository.RunPushNotificationTask(pushQueue);

        logger.LogInformation($"Đã tạo yêu cầu mở cửa {requestId} và gửi thông báo cho Manager {entity.ManagerId}");

        return requestId;
    }
}