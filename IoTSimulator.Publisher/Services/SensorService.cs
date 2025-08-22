using IoTSimulator.Publisher.Models;

namespace IoTSimulator.Publisher.Services;

public class SensorService : ISensorService
{
    private const int DecimalPlaces = 1;
    private const double MinTemperature = 10.0;
    private const double MaxTemperature = 40.0;
    private const double MinHumidity = 20.0;
    private const double MaxHumidity = 90.0;

    private readonly Random _random;
    private readonly List<SensorConfiguration> _sensorConfigurations;
    private readonly string[] _possibleLocations = 
    {
        "Living Room", "Kitchen", "Bedroom", "Bathroom", "Garage", "Garden", 
        "Office", "Basement", "Attic", "Balcony", "Dining Room", "Study Room"
    };

    public SensorService()
    {
        _random = new Random();
        _sensorConfigurations = new List<SensorConfiguration>();
        
        InitializeDefaultSensors();
    }

    public SensorData GenerateData(SensorConfiguration sensorConfig)
    {
        ArgumentNullException.ThrowIfNull(sensorConfig);

        var temperature = GenerateRandomValue(MinTemperature, MaxTemperature);
        var humidity = GenerateRandomValue(MinHumidity, MaxHumidity);
        var randomLocation = _possibleLocations[_random.Next(_possibleLocations.Length)];

        return new SensorData
        {
            SensorId = sensorConfig.SensorId,
            SensorName = sensorConfig.SensorName,
            Location = randomLocation,
            Temperature = temperature,
            Humidity = humidity,
            Timestamp = DateTime.UtcNow
        };
    }

    public IEnumerable<SensorData> GenerateDataForAllSensors()
    {
        return _sensorConfigurations.Select(GenerateData);
    }

    public IEnumerable<SensorConfiguration> GetSensorConfigurations()
    {
        return _sensorConfigurations.AsReadOnly();
    }

    public void AddSensor(SensorConfiguration sensorConfig)
    {
        ArgumentNullException.ThrowIfNull(sensorConfig);

        if (string.IsNullOrWhiteSpace(sensorConfig.SensorId))
        {
            throw new ArgumentException("SensorId cannot be null or empty.", nameof(sensorConfig));
        }

        if (_sensorConfigurations.Any(s => s.SensorId == sensorConfig.SensorId))
        {
            throw new ArgumentException($"A sensor with ID '{sensorConfig.SensorId}' already exists.", nameof(sensorConfig));
        }

        _sensorConfigurations.Add(sensorConfig);
    }

    public bool RemoveSensor(string sensorId)
    {
        if (string.IsNullOrWhiteSpace(sensorId))
        {
            return false;
        }

        var sensorToRemove = _sensorConfigurations.FirstOrDefault(s => s.SensorId == sensorId);
        if (sensorToRemove != null)
        {
            return _sensorConfigurations.Remove(sensorToRemove);
        }

        return false;
    }

    private void InitializeDefaultSensors()
    {
        var defaultSensors = new[]
        {
            new SensorConfiguration
            {
                SensorId = "SENSOR_001",
                SensorName = "Primary Environmental Sensor"
            },
            new SensorConfiguration
            {
                SensorId = "SENSOR_002", 
                SensorName = "Secondary Climate Monitor"
            },
            new SensorConfiguration
            {
                SensorId = "SENSOR_003",
                SensorName = "Backup Temperature Gauge"
            },
            new SensorConfiguration
            {
                SensorId = "SENSOR_004",
                SensorName = "Mobile Weather Station"
            },
            new SensorConfiguration
            {
                SensorId = "SENSOR_005",
                SensorName = "Smart Environment Tracker"
            }
        };

        foreach (var sensor in defaultSensors)
        {
            _sensorConfigurations.Add(sensor);
        }
    }

    private double GenerateRandomValue(double min, double max)
    {
        var value = _random.NextDouble() * (max - min) + min;
        return Math.Round(value, DecimalPlaces);
    }
}
