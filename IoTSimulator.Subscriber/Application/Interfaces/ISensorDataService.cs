using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Interfaces;

public interface ISensorDataService
{
    // Original entity methods (kept for backward compatibility)
    Task<SensorData> CreateSensorDataAsync(SensorData sensorData);
    Task<SensorData?> GetSensorDataByIdAsync(Guid id);
    Task<IEnumerable<SensorData>> GetAllSensorDataAsync();
    Task<IEnumerable<SensorData>> GetSensorDataByDeviceIdAsync(Guid deviceId);
    Task<IEnumerable<SensorData>> GetSensorDataByDeviceIdAndDateRangeAsync(Guid deviceId, DateTime startDate, DateTime endDate);
    Task<SensorData?> GetLatestSensorDataByDeviceIdAsync(Guid deviceId);
    Task<SensorData> CreateSensorDataFromMqttAsync(string sensorId, string? sensorName, string? location, double? temperature, double? humidity, DateTime timestamp);
    Task<SensorData> UpdateSensorDataAsync(SensorData sensorData);
    Task<bool> DeleteSensorDataAsync(Guid id);
    Task<int> DeleteOldSensorDataAsync(DateTime olderThan);
    Task<SensorDataAggregate> GetAggregatedDataAsync(Guid deviceId, DateTime startTime, DateTime endTime);

    // New DTO-based methods
    Task<SensorDataDto> CreateSensorDataAsync(CreateSensorDataDto command);
    Task<SensorDataDto?> GetSensorDataDtoByIdAsync(Guid id);
    Task<IEnumerable<SensorDataDto>> GetAllSensorDataDtoAsync();
    Task<IEnumerable<SensorDataDto>> GetSensorDataDtoByDeviceIdAsync(Guid deviceId);
    Task<IEnumerable<SensorDataDto>> GetSensorDataDtoByDeviceIdAndDateRangeAsync(Guid deviceId, DateTime startDate, DateTime endDate);
    Task<SensorDataDto?> GetLatestSensorDataDtoByDeviceIdAsync(Guid deviceId);
    Task<SensorDataDto> UpdateSensorDataAsync(UpdateSensorDataDto command);
}
