using System.Text.Json;
using IoTSimulator.Publisher.Models;
using MQTTnet;

namespace IoTSimulator.Publisher.Services;

public class MqttPublisherService : IMqttPublisherService
{
    private const string BrokerHost = "localhost";
    private const int BrokerPort = 1883;
    private const string TopicName = "sensor/data";

    private readonly IMqttClient _mqttClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed = false;

    public MqttPublisherService()
    {
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public bool IsConnected => _mqttClient.IsConnected;

    public async Task InitializeAsync()
    {
        var options = BuildMqttClientOptions();
        await _mqttClient.ConnectAsync(options, CancellationToken.None);
    }

    public async Task PublishAsync(SensorData data, string? topic = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!IsConnected)
        {
            throw new InvalidOperationException("MQTT client is not connected. Call InitializeAsync first.");
        }

        var topicToUse = topic ?? TopicName;
        var jsonPayload = SerializeSensorData(data);
        var message = BuildMqttMessage(topicToUse, jsonPayload);

        await _mqttClient.PublishAsync(message, CancellationToken.None);
    }

    public async Task PublishBatchAsync(IEnumerable<SensorData> sensorDataCollection)
    {
        ArgumentNullException.ThrowIfNull(sensorDataCollection);

        if (!IsConnected)
        {
            throw new InvalidOperationException("MQTT client is not connected. Call InitializeAsync first.");
        }

        var publishTasks = sensorDataCollection.Select(sensorData =>
            PublishAsync(sensorData, DetermineTopicForSensor(sensorData)));

        await Task.WhenAll(publishTasks);
    }

    private static MqttClientOptions BuildMqttClientOptions()
    {
        return new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerHost, BrokerPort)
            .WithClientId(Guid.NewGuid().ToString())
            .WithCleanSession()
            .WithCredentials("", "") // Anonymous authentication - empty username and password
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311) // Use MQTT v3.1.1 for better compatibility
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .WithTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    private string SerializeSensorData(SensorData data)
    {
        return JsonSerializer.Serialize(data, _jsonOptions);
    }

    private static MqttApplicationMessage BuildMqttMessage(string topic, string payload)
    {
        return new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
    }

    private static string DetermineTopicForSensor(SensorData sensorData)
    {
        return string.IsNullOrWhiteSpace(sensorData.SensorId) 
            ? TopicName 
            : $"sensors/{sensorData.SensorId.ToLowerInvariant()}/data";
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (_mqttClient.IsConnected)
            {
                _mqttClient.DisconnectAsync().GetAwaiter().GetResult();
            }
            _mqttClient?.Dispose();
            _disposed = true;
        }
    }
}
