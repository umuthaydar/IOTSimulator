using IoTSimulator.Publisher.Models;
using IoTSimulator.Publisher.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace IoTSimulator.Publisher;

public class Program
{
    private const int PublishIntervalMs = 1000;
    
    public static async Task Main(string[] args)
    {
        Console.WriteLine("IoT Publisher started");

        var serviceProvider = ConfigureServices();

        try
        {
            await RunApplicationAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex.Message}");
        }
        finally
        {
            await serviceProvider.DisposeAsync();
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<ISensorService, SensorService>();
        services.AddSingleton<IMqttPublisherService, MqttPublisherService>();

        return services.BuildServiceProvider();
    }

    private static async Task RunApplicationAsync(ServiceProvider serviceProvider)
    {
        var sensorService = serviceProvider.GetRequiredService<ISensorService>();
        var mqttPublisher = serviceProvider.GetRequiredService<IMqttPublisherService>();

        try
        {
            await InitializeMqttConnectionAsync(mqttPublisher);
            await PublishSensorDataContinuouslyAsync(sensorService, mqttPublisher);
        }
        finally
        {
            mqttPublisher.Dispose();
        }
    }

    private static async Task InitializeMqttConnectionAsync(IMqttPublisherService mqttPublisher)
    {
        try
        {
            await mqttPublisher.InitializeAsync();
            Console.WriteLine("Connected to MQTT broker on localhost:1883");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to MQTT broker: {ex.Message}");
            throw;
        }
    }

    private static async Task PublishSensorDataContinuouslyAsync(ISensorService sensorService, IMqttPublisherService mqttPublisher)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
            Console.WriteLine("\nShutdown requested...");
        };

        Console.WriteLine("Publishing sensor data. Press Ctrl+C to stop.");

        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await PublishSingleSensorReadingAsync(sensorService, mqttPublisher);
                await Task.Delay(PublishIntervalMs, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing data: {ex.Message}");
                await Task.Delay(PublishIntervalMs, cancellationTokenSource.Token);
            }
        }
    }

    private static async Task PublishSingleSensorReadingAsync(ISensorService sensorService, IMqttPublisherService mqttPublisher)
    {
        var allSensorData = sensorService.GenerateDataForAllSensors().ToList();
        
        if (!allSensorData.Any())
        {
            Console.WriteLine("⚠️  No sensors configured. Skipping data generation.");
            return;
        }

        await mqttPublisher.PublishBatchAsync(allSensorData);

        Console.WriteLine($"📊 [{DateTime.Now:HH:mm:ss}] Published data from {allSensorData.Count} sensors:");
        foreach (var sensorData in allSensorData)
        {
            Console.WriteLine($"   🌡️  [{sensorData.SensorId}] {sensorData.Location}: {sensorData.Temperature:F1}°C, {sensorData.Humidity:F1}% - Topic: sensors/{sensorData.SensorId.ToLower()}/data");
        }
    }
}
