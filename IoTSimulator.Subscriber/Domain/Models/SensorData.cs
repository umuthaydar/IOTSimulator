using IoTSimulator.Subscriber.Domain.Common;

namespace IoTSimulator.Subscriber.Domain.Models;

public class SensorData : BaseEntity
{
    public Guid DeviceId { get; set; }

    public virtual IoTDevice Device { get; set; } = null!;

    public string? SensorId { get; set; }

    public string? SensorName { get; set; }

    public string? Location { get; set; }

    public double? Temperature { get; set; }

    public double? Humidity { get; set; }

    public DateTime Timestamp { get; set; }

    public string? Metadata { get; set; }
}
