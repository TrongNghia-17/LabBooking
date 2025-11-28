namespace LabBooking.Application.Features.Equipments.Dtos;

public class EquipmentSimpleResponse
{
    public Guid Id { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string LabRoomName { get; set; } = string.Empty;
}
