using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAvailableLabsByDate;

public class GetAvailableLabsByDateQueryHandler(
    ILabRoomRepository labRoomRepository,
    ILogger<GetAvailableLabsByDateQueryHandler> logger) : IRequestHandler<GetAvailableLabsByDateQuery, IEnumerable<LabRoomAvailabilityDto>>
{
    public async Task<IEnumerable<LabRoomAvailabilityDto>> Handle(GetAvailableLabsByDateQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang kiểm tra phòng trống cho ngày: {Date}", request.Date);

        // Gọi Repository (Hàm này bạn đã implement ở bước trước)
        var domainModels = await labRoomRepository.GetAvailableLabsByDateAsync(request.Date, cancellationToken);

        var dtos = domainModels.Select(m => new LabRoomAvailabilityDto
        {
            LabId = m.LabId,
            LabName = m.LabName,
            Location = m.Location,
            Capacity = m.Capacity,
            AvailableSlotIds = m.AvailableSlots.Select(s => s.Id).ToList(),
            AvailableSlots = m.AvailableSlots.Select(s => new SlotDto
            {
                Id = s.Id,
                SlotIndex = s.SlotIndex,
                // Format TimeOnly thành string "HH:mm" tại đây (Tầng Application lo việc hiển thị)
                StartTime = s.StartTime.ToString("HH:mm"),
                EndTime = s.EndTime.ToString("HH:mm")
            }).ToList()
        });

        return dtos;
    }
}