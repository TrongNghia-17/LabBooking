namespace LabBooking.Application.Features.Managers.Dtos;

public class EquipmentCategoryGroupDto
{
    public string CategoryName { get; set; } = string.Empty; // Ví dụ: "Thiết bị IoT"
    public int TotalCount { get; set; } // Tổng số lượng trong nhóm (Optional - để hiển thị cho đẹp)
    public List<ManagerEquipmentDto> Items { get; set; } = new(); // Danh sách thiết bị con (IoT_A, IoT_B)
}

public class ManagerLabDetailsResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ManagerLabRoomDto> ManagedLabs { get; set; } = new();
}

public class ManagerLabRoomDto
{
    public Guid Id { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int? MaximumLimit { get; set; }
    public string Status { get; set; } = "Available";

    public List<EquipmentCategoryGroupDto> EquipmentGroups { get; set; } = new();
}

public class ManagerEquipmentDto
{
    public Guid Id { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Available";
    public bool IsAvailable { get; set; }
}

