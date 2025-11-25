using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.LabRooms.Dtos;

//public record LabRoomResponse(
//    Guid Id,
//    string? LabName,
//    string? Location,
//    int? MaximumLimit,
//    Guid? MainManagerId,
//    Guid? CreatedById,
//    DateTime CreatedDate,
//    bool IsActive,
//    ICollection<EquipmentResponse>? Equipments
//);

public class LabRoomResponse
{
    public Guid Id { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int? MaximumLimit { get; set; }
    public Guid? MainManagerId { get; set; }
    public string? MainManagerName { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public ICollection<EquipmentResponse>? Equipments { get; set; }
    public string Status { get; set; } = "Available"; // "Available", "Booked", "Maintenance"
}
