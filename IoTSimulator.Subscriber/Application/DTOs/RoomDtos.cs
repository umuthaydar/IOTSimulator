namespace IoTSimulator.Subscriber.Application.DTOs;

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public Guid HouseId { get; set; }
}

public class UpdateRoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid HouseId { get; set; }
}

public class RoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid HouseId { get; set; }
    public string HouseName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class RoomWithDevicesDto : RoomDto
{
    public List<IoTDeviceDto> Devices { get; set; } = new();
}
