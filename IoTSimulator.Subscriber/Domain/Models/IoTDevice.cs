using IoTSimulator.Subscriber.Domain.Common;
using IoTSimulator.Subscriber.Domain.Enums;

namespace IoTSimulator.Subscriber.Domain.Models;

public class IoTDevice : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid? RoomId { get; set; }

    public virtual Room? Room { get; set; }

    public DeviceType DeviceType { get; set; }

    public string? Manufacturer { get; set; }

    public string? Model { get; set; }

    public string? SerialNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<SensorData> SensorDataList { get; set; } = new List<SensorData>();
}
