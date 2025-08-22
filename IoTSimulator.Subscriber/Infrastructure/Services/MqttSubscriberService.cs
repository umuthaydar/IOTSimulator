using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using System.Text;
using System.Text.Json;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Repositories;
using IoTSimulator.Subscriber.Hubs;

namespace IoTSimulator.Subscriber.Infrastructure.Services;

public class MqttSubscriberService : BackgroundService
{
    private readonly ILogger<MqttSubscriberService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<SensorDataHub> _hubContext;
    private IMqttClient? _mqttClient;
    private bool _disposed = false;

    public MqttSubscriberService(
        ILogger<MqttSubscriberService> logger,
        IServiceProvider serviceProvider,
        IHubContext<SensorDataHub> hubContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting MQTT Subscriber Service...");

            var clientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) // Default MQTT broker settings
                .WithClientId($"IoTSubscriber_{Environment.MachineName}_{Guid.NewGuid():N}")
                .WithCleanSession(true);

            _mqttClient = new MqttClientFactory().CreateMqttClient();

            _mqttClient.ConnectedAsync += OnMqttClientConnected;
            _mqttClient.DisconnectedAsync += OnMqttClientDisconnected;
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

            await _mqttClient.ConnectAsync(clientOptionsBuilder.Build(), stoppingToken);

            _logger.LogInformation("MQTT Subscriber Service started successfully.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("MQTT Subscriber Service stopping...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run MQTT Subscriber Service");
            throw;
        }
        finally
        {
            await StopMqttClientAsync();
        }
    }

    public async Task StartAsync()
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        await StopMqttClientAsync();
    }

    private async Task StopMqttClientAsync()
    {
        try
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                _logger.LogInformation("Stopping MQTT Subscriber Service...");
                await _mqttClient.DisconnectAsync();
                _logger.LogInformation("MQTT Subscriber Service stopped successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping MQTT Subscriber Service");
        }
    }

    private async Task OnMqttClientConnected(MqttClientConnectedEventArgs eventArgs)
    {
        try
        {
            _logger.LogInformation("Connected to MQTT broker successfully.");

            // Subscribe to sensor data topics
            var subscribeOptionsBuilder = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f
                    .WithTopic("sensors/+/data") // Subscribe to all sensor data topics
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce));

            await _mqttClient!.SubscribeAsync(subscribeOptionsBuilder.Build());
            _logger.LogInformation("Subscribed to topic: sensors/+/data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MQTT client connection setup");
        }
    }

    private Task OnMqttClientDisconnected(MqttClientDisconnectedEventArgs eventArgs)
    {
        _logger.LogWarning("Disconnected from MQTT broker. Reason: {Reason}", eventArgs.Reason);
        
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                if (_mqttClient != null && !_mqttClient.IsConnected)
                {
                    _logger.LogInformation("Attempting to reconnect to MQTT broker...");
                    await _mqttClient.ConnectAsync(_mqttClient.Options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reconnect to MQTT broker");
            }
        });
        
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        try
        {
            var topic = eventArgs.ApplicationMessage.Topic;
            var payload = eventArgs.ApplicationMessage.ConvertPayloadToString();

            _logger.LogDebug("Received message on topic: {Topic}, Payload: {Payload}", topic, payload);

            var topicParts = topic.Split('/');
            if (topicParts.Length >= 3 && topicParts[0] == "sensors" && topicParts[2] == "data")
            {
                var sensorId = topicParts[1];
                await ProcessSensorData(sensorId, payload);
            }
            else
            {
                _logger.LogWarning("Invalid topic format: {Topic}", topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MQTT message");
        }
    }

    private async Task ProcessSensorData(string sensorId, string payload)
    {
        try
        {
            var sensorData = JsonSerializer.Deserialize<SensorDataPayload>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (sensorData == null)
            {
                _logger.LogWarning("Failed to deserialize sensor data for sensor {SensorId}", sensorId);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var deviceRepository = scope.ServiceProvider.GetRequiredService<DeviceRepository>();
            var sensorDataRepository = scope.ServiceProvider.GetRequiredService<SensorDataRepository>();

            var device = await deviceRepository.FindBySensorIdAsync(sensorId);
            if (device == null)
            {
                var location = $"{sensorData.Latitude:F6},{sensorData.Longitude:F6}";
                device = await deviceRepository.CreateDefaultDeviceAsync(sensorId, sensorData.Name ?? $"Device_{sensorId}", location);
                _logger.LogInformation("Created new device for sensor {SensorId}", sensorId);
            }

            // Cihaz aktif değilse veri işleme ve SignalR gönderimini durdur
            if (!device.IsActive)
            {
                _logger.LogDebug("Device {DeviceId} is inactive, skipping sensor data processing and SignalR broadcast", device.Id);
                
                // Cihaz inaktif durumunu SignalR ile bildir
                var disconnectedData = new
                {
                    DeviceId = device.Id,
                    RoomId = device.RoomId,
                    HouseId = device.Room?.HouseId,
                    SensorId = sensorId,
                    DeviceName = device.Name,
                    RoomName = device.Room?.Name,
                    Status = "Disconnected",
                    IsActive = false,
                    Timestamp = DateTime.UtcNow
                };

                if (device.RoomId != null)
                {
                    await _hubContext.SendDeviceStatusUpdate(device.RoomId.Value.ToString(), disconnectedData);
                }
                
                if (device.Room?.HouseId != null)
                {
                    await _hubContext.SendDeviceStatusUpdate(device.Room.HouseId.ToString(), disconnectedData);
                }
                
                return; // Veri işlemeyi durdur
            }

            var sensorDataEntity = new SensorData
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                SensorId = sensorId,
                SensorName = sensorData.Name,
                Location = $"{sensorData.Latitude:F6},{sensorData.Longitude:F6}",
                Temperature = sensorData.Temperature,
                Humidity = sensorData.Humidity,
                Timestamp = sensorData.Timestamp,
                CreatedAt = DateTime.UtcNow
            };

            await sensorDataRepository.AddAsync(sensorDataEntity);

            var sensorUpdateData = new
            {
                DeviceId = device.Id,
                RoomId = device.RoomId,
                HouseId = device.Room?.HouseId,
                SensorId = sensorId,
                Temperature = sensorData.Temperature,
                Humidity = sensorData.Humidity,
                Timestamp = sensorData.Timestamp,
                DeviceName = device.Name,
                RoomName = device.Room?.Name
            };

            if (device.RoomId != null)
            {
                await _hubContext.SendSensorDataToRoom(device.RoomId.Value.ToString(), sensorUpdateData);
            }

            if (device.Room?.HouseId != null)
            {
                await _hubContext.SendSensorDataToHouse(device.Room.HouseId.ToString(), sensorUpdateData);
            }

            _logger.LogInformation("Persisted sensor data for device {DeviceId} (Sensor: {SensorId}) at {Timestamp}", 
                device.Id, sensorId, sensorData.Timestamp);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON payload for sensor {SensorId}: {Payload}", sensorId, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sensor data for sensor {SensorId}", sensorId);
        }
    }

    public override void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                if (_mqttClient?.IsConnected == true)
                {
                    _mqttClient.DisconnectAsync().Wait(TimeSpan.FromSeconds(5));
                }
                _mqttClient?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing MQTT client");
            }
            finally
            {
                _disposed = true;
            }
        }
        
        base.Dispose();
    }

    private class SensorDataPayload
    {
        public string? Name { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
