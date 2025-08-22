namespace IoTSimulator.Publisher.Models;

public class SensorData
{
    public string SensorId { get; set; } = string.Empty;

    public string SensorName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public DateTime Timestamp { get; set; }
}
