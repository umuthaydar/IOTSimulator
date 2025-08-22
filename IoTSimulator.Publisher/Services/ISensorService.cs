using IoTSimulator.Publisher.Models;

namespace IoTSimulator.Publisher.Services;

public interface ISensorService
{
    SensorData GenerateData(SensorConfiguration sensorConfig);

    IEnumerable<SensorData> GenerateDataForAllSensors();

    IEnumerable<SensorConfiguration> GetSensorConfigurations();

    void AddSensor(SensorConfiguration sensorConfig);

    bool RemoveSensor(string sensorId);
}
