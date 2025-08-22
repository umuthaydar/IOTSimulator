using IoTSimulator.Subscriber.Domain.Enums;

namespace IoTSimulator.Subscriber.Application.DTOs;

public class CreateIoTDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public DeviceType DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateIoTDeviceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public DeviceType DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public bool IsActive { get; set; } = true;
}

public class IoTDeviceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public RoomDto? Room { get; set; } // Room navigation property
    public DeviceType DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
