namespace IoTSimulator.Subscriber.Application.DTOs;

public class CreateSensorDataDto
{
    public Guid DeviceId { get; set; }
    public string? SensorId { get; set; }
    public string? SensorName { get; set; }
    public string? Location { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Metadata { get; set; }
}

public class UpdateSensorDataDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string? SensorId { get; set; }
    public string? SensorName { get; set; }
    public string? Location { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Metadata { get; set; }
}

public class SensorDataDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string? SensorId { get; set; }
    public string? SensorName { get; set; }
    public string? Location { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Metadata { get; set; }
}

public class CreateSensorDataFromMqttDto
{
    public string SensorId { get; set; } = string.Empty;
    public string? SensorName { get; set; }
    public string? Location { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}
