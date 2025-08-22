using IoTSimulator.Publisher.Models;

namespace IoTSimulator.Publisher.Services;

public interface IMqttPublisherService : IDisposable
{
    Task InitializeAsync();

    Task PublishAsync(SensorData data, string? topic = null);

    Task PublishBatchAsync(IEnumerable<SensorData> sensorDataCollection);

    bool IsConnected { get; }
}
