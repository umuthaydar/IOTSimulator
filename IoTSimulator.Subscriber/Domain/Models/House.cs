using IoTSimulator.Subscriber.Domain.Common;

namespace IoTSimulator.Subscriber.Domain.Models;

public class House : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
