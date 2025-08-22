namespace IoTSimulator.Subscriber.Application.DTOs;

public class SensorDataAggregate
{
    public Guid DeviceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Count { get; set; }
    public double? AverageTemperature { get; set; }
    public double? MinTemperature { get; set; }
    public double? MaxTemperature { get; set; }
    public double? AverageHumidity { get; set; }
    public double? MinHumidity { get; set; }
    public double? MaxHumidity { get; set; }
}
