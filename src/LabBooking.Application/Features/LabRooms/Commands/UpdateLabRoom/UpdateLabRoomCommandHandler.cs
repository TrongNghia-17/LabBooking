using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.LabRooms.Commands.UpdateLabRoom;

public class UpdateLabRoomCommandHandler(
    ILogger<UpdateLabRoomCommandHandler> logger,
    IMapper mapper,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateLabRoomCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLabRoomCommand request, CancellationToken cancellationToken)
    {
        var updaterId = currentUserService.UserId;

        if (updaterId == null)
        {
            logger.LogWarning("Không tìm thấy thông tin người dùng (chưa đăng nhập).");
            throw new UnauthorizedAccessException("Người dùng không được xác thực.");
        }

        logger.LogInformation("Người dùng {UpdaterId} đang cập nhật phòng lab {LabRoomId}", updaterId.Value, request.Id);

        var labRoomToUpdate = await labRoomRepository.GetByIdAsync(request.Id);

        if (labRoomToUpdate == null)
        {
            logger.LogWarning("Lab room with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(LabRoom), request.Id.ToString());
        }

        mapper.Map(request, labRoomToUpdate);

        labRoomToUpdate.LastUpdatedDate = DateTime.UtcNow;

        await labRoomRepository.Update(labRoomToUpdate, cancellationToken);

        return Unit.Value;
    }
}
