using IoTSimulator.Subscriber.Domain.Common;

namespace IoTSimulator.Subscriber.Domain.Models;

public class Room : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid HouseId { get; set; }

    public virtual House House { get; set; } = null!;

    public virtual ICollection<IoTDevice> Devices { get; set; } = new List<IoTDevice>();
}
