using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;

public class UpdateDoorRequestStatusHandler(
    IDoorRequestRepository doorRequestRepository,
    INotificationRepository notificationRepository,
    ICurrentUserService currentUserService,
    UserManager<User> userManager,
    ILogger<UpdateDoorRequestStatusHandler> logger
    ) : IRequestHandler<UpdateDoorRequestStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDoorRequestStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start processing door request. RequestId: {RequestId}, NewStatus: {Status}", request.Id, request.NewStatus);

        // 1. Get Request via Repository
        var doorRequest = await doorRequestRepository.GetByIdAsync(request.Id);
        if (doorRequest == null)
        {
            logger.LogWarning("Door request not found. RequestId: {RequestId}", request.Id);
            throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());
        }

        // 2. Validate Business Logic (Cannot process already processed request)
        if (doorRequest.Status != DoorRequestStatus.Pending)
        {
            logger.LogWarning("Attempt to process a non-pending request. RequestId: {RequestId}, CurrentStatus: {Status}", request.Id, doorRequest.Status);
            throw new BadRequestException("Yêu cầu này đã được xử lý trước đó, không thể thay đổi.");
        }

        // 3. Get Current Manager Info
        var managerId = currentUserService.UserId;
        var manager = await userManager.FindByIdAsync(managerId.ToString());

        if (manager == null)
        {
            logger.LogError("Manager user not found in system but passed auth check. ManagerId: {ManagerId}", managerId);
            throw new UnauthorizedAccessException("Không tìm thấy thông tin quản lý.");
        }

        // 4. Update Entity (Using Domain Method - Clean Code)
        // Logic gán ngày giờ, gán người duyệt nằm gọn trong Entity
        doorRequest.Process(manager, request.NewStatus, request.Note);

        // 5. Save Changes via Repository
        await doorRequestRepository.UpdateAsync(doorRequest);

        logger.LogInformation("Successfully updated door request status. RequestId: {RequestId}, Status: {Status}", request.Id, request.NewStatus);

        // 6. GỬI THÔNG BÁO CHO USER (Người tạo yêu cầu)
        var pushQueue = new List<PushNotificationData>();
        Notification? notiEntity = null;

        // Kiểm tra trạng thái để gửi notification phù hợp
        if (request.NewStatus == DoorRequestStatus.Accepted)
        {
            // ĐƯỢC DUYỆT
            var (entity, pushData) = notificationRepository.PrepareNotification(
                doorRequest.RequestedById,
                "✅ Yêu cầu mở cửa được duyệt",
                $"Yêu cầu mở cửa cho mã đặt phòng {doorRequest.BookingCode} đã được chấp nhận. {request.Note}",
                "DOOR_REQUEST_ACCEPTED",
                new
                {
                    requestId = request.Id,
                    bookingCode = doorRequest.BookingCode,
                    status = "Accepted",
                    note = request.Note,
                    processedBy = manager.FullName,
                    processedAt = DateTime.UtcNow
                }
            );
            notiEntity = entity;
            pushQueue.Add(pushData);

            logger.LogInformation("Notification prepared for accepted request. UserId: {UserId}", doorRequest.RequestedById);
        }
        else if (request.NewStatus == DoorRequestStatus.Rejected)
        {
            // BỊ TỪ CHỐI
            var (entity, pushData) = notificationRepository.PrepareNotification(
                doorRequest.RequestedById,
                "⛔ Yêu cầu mở cửa bị từ chối",
                $"Yêu cầu mở cửa cho mã đặt phòng {doorRequest.BookingCode} đã bị từ chối. Lý do: {request.Note}",
                "DOOR_REQUEST_REJECTED",
                new
                {
                    requestId = request.Id,
                    bookingCode = doorRequest.BookingCode,
                    status = "Rejected",
                    reason = request.Note,
                    processedBy = manager.FullName,
                    processedAt = DateTime.UtcNow
                }
            );
            notiEntity = entity;
            pushQueue.Add(pushData);

            logger.LogInformation("Notification prepared for rejected request. UserId: {UserId}", doorRequest.RequestedById);
        }

        if (notiEntity != null)
        {
            await notificationRepository.CreateAsync(notiEntity, cancellationToken);
        }

        // Gửi push notification (background task)
        if (pushQueue.Any())
        {
            notificationRepository.RunPushNotificationTask(pushQueue);
        }

        return Unit.Value;
    }
}
