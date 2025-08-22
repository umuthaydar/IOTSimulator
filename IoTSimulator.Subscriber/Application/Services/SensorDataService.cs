using AutoMapper;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Repositories;
using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Services;

public class SensorDataService : ISensorDataService
{
    private readonly SensorDataRepository _sensorDataRepository;
    private readonly DeviceRepository _deviceRepository;
    private readonly IMapper _mapper;

    public SensorDataService(SensorDataRepository sensorDataRepository, DeviceRepository deviceRepository, IMapper mapper)
    {
        _sensorDataRepository = sensorDataRepository ?? throw new ArgumentNullException(nameof(sensorDataRepository));
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<SensorData> CreateSensorDataAsync(SensorData sensorData)
    {
        if (sensorData == null)
            throw new ArgumentNullException(nameof(sensorData));

        if (sensorData.DeviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(sensorData));

        var device = await _deviceRepository.GetByIdAsync(sensorData.DeviceId);
        if (device == null)
            throw new ArgumentException($"Device with ID {sensorData.DeviceId} does not exist.", nameof(sensorData));

        if (sensorData.Timestamp == DateTime.MinValue)
            sensorData.Timestamp = DateTime.UtcNow;

        return await _sensorDataRepository.AddAsync(sensorData);
    }


    public async Task<IEnumerable<SensorData>> GetAllSensorDataAsync()
    {
        return await _sensorDataRepository.GetAllAsync();
    }


    public async Task<SensorData?> GetSensorDataByIdAsync(Guid id)
    {
        return await _sensorDataRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<SensorData>> GetSensorDataByDeviceIdAsync(Guid deviceId)
    {
        return await _sensorDataRepository.GetByDeviceIdAsync(deviceId);
    }

    public async Task<IEnumerable<SensorData>> GetSensorDataByDeviceIdAndDateRangeAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be earlier than end date.");

        return await _sensorDataRepository.GetByDeviceIdAndDateRangeAsync(deviceId, startDate, endDate);
    }

    public async Task<SensorData?> GetLatestSensorDataByDeviceIdAsync(Guid deviceId)
    {
        return await _sensorDataRepository.GetLatestByDeviceIdAsync(deviceId);
    }

    public async Task<SensorData> CreateSensorDataFromMqttAsync(string sensorId, string? sensorName, string? location, double? temperature, double? humidity, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(sensorId))
            throw new ArgumentException("Sensor ID is required.", nameof(sensorId));

        var device = await _deviceRepository.FindBySensorIdAsync(sensorId);
        if (device == null)
        {
            throw new ArgumentException($"Device with sensor ID '{sensorId}' not found. Please ensure the device is registered first.", nameof(sensorId));
        }

        var sensorData = new SensorData
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            Temperature = temperature,
            Humidity = humidity,
            Timestamp = timestamp
        };

        return await _sensorDataRepository.AddAsync(sensorData);
    }


    public async Task<SensorData> UpdateSensorDataAsync(SensorData sensorData)
    {
        if (sensorData == null)
            throw new ArgumentNullException(nameof(sensorData));

        if (sensorData.Id == Guid.Empty)
            throw new ArgumentException("Sensor data ID is required for update.", nameof(sensorData));

        var existingSensorData = await _sensorDataRepository.GetByIdAsync(sensorData.Id);
        if (existingSensorData == null)
            throw new ArgumentException($"Sensor data with ID {sensorData.Id} does not exist.", nameof(sensorData));

        if (sensorData.DeviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(sensorData));

        var device = await _deviceRepository.GetByIdAsync(sensorData.DeviceId);
        if (device == null)
            throw new ArgumentException($"Device with ID {sensorData.DeviceId} does not exist.", nameof(sensorData));

        return await _sensorDataRepository.UpdateAsync(sensorData);
    }


    public async Task<bool> DeleteSensorDataAsync(Guid id)
    {
        var sensorData = await _sensorDataRepository.GetByIdAsync(id);
        if (sensorData == null)
            return false;

        return await _sensorDataRepository.DeleteAsync(id);
    }


    public async Task<SensorDataAggregate> GetAggregatedDataAsync(Guid deviceId, DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be earlier than end time.");

        var sensorData = await _sensorDataRepository.GetByDeviceIdAndDateRangeAsync(deviceId, startTime, endTime);
        var dataList = sensorData.ToList();

        if (!dataList.Any())
        {
            return new SensorDataAggregate
            {
                DeviceId = deviceId,
                StartTime = startTime,
                EndTime = endTime,
                Count = 0,
                AverageTemperature = null,
                MinTemperature = null,
                MaxTemperature = null,
                AverageHumidity = null,
                MinHumidity = null,
                MaxHumidity = null
            };
        }

        var temperatureValues = dataList.Where(x => x.Temperature.HasValue).Select(x => x.Temperature!.Value).ToList();
        var humidityValues = dataList.Where(x => x.Humidity.HasValue).Select(x => x.Humidity!.Value).ToList();

        return new SensorDataAggregate
        {
            DeviceId = deviceId,
            StartTime = startTime,
            EndTime = endTime,
            Count = dataList.Count,
            AverageTemperature = temperatureValues.Any() ? temperatureValues.Average() : null,
            MinTemperature = temperatureValues.Any() ? temperatureValues.Min() : null,
            MaxTemperature = temperatureValues.Any() ? temperatureValues.Max() : null,
            AverageHumidity = humidityValues.Any() ? humidityValues.Average() : null,
            MinHumidity = humidityValues.Any() ? humidityValues.Min() : null,
            MaxHumidity = humidityValues.Any() ? humidityValues.Max() : null
        };
    }


    public async Task<int> DeleteOldSensorDataAsync(DateTime olderThan)
    {
        return await _sensorDataRepository.DeleteOldDataAsync(olderThan);
    }


    public async Task<int> GetSensorDataCountAsync(Guid deviceId)
    {
        var sensorData = await _sensorDataRepository.GetByDeviceIdAsync(deviceId);
        return sensorData.Count();
    }

    // DTO-based methods
    public async Task<SensorDataDto> CreateSensorDataAsync(CreateSensorDataDto command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.DeviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(command));

        var device = await _deviceRepository.GetByIdAsync(command.DeviceId);
        if (device == null)
            throw new ArgumentException($"Device with ID {command.DeviceId} does not exist.", nameof(command));

        var sensorData = _mapper.Map<SensorData>(command);
        if (sensorData.Timestamp == DateTime.MinValue)
            sensorData.Timestamp = DateTime.UtcNow;

        var createdSensorData = await _sensorDataRepository.AddAsync(sensorData);
        
        return _mapper.Map<SensorDataDto>(createdSensorData);
    }

    public async Task<SensorDataDto?> GetSensorDataDtoByIdAsync(Guid id)
    {
        var sensorData = await _sensorDataRepository.GetByIdAsync(id);
        return sensorData != null ? _mapper.Map<SensorDataDto>(sensorData) : null;
    }

    public async Task<IEnumerable<SensorDataDto>> GetAllSensorDataDtoAsync()
    {
        var sensorData = await _sensorDataRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<SensorDataDto>>(sensorData);
    }

    public async Task<IEnumerable<SensorDataDto>> GetSensorDataDtoByDeviceIdAsync(Guid deviceId)
    {
        var sensorData = await _sensorDataRepository.GetByDeviceIdAsync(deviceId);
        return _mapper.Map<IEnumerable<SensorDataDto>>(sensorData);
    }

    public async Task<IEnumerable<SensorDataDto>> GetSensorDataDtoByDeviceIdAndDateRangeAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        var sensorData = await _sensorDataRepository.GetByDeviceIdAndDateRangeAsync(deviceId, startDate, endDate);
        return _mapper.Map<IEnumerable<SensorDataDto>>(sensorData);
    }

    public async Task<SensorDataDto?> GetLatestSensorDataDtoByDeviceIdAsync(Guid deviceId)
    {
        var sensorData = await _sensorDataRepository.GetLatestByDeviceIdAsync(deviceId);
        return sensorData != null ? _mapper.Map<SensorDataDto>(sensorData) : null;
    }

    public async Task<SensorDataDto> UpdateSensorDataAsync(UpdateSensorDataDto command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Id == Guid.Empty)
            throw new ArgumentException("SensorData ID is required for update.", nameof(command));

        var existingSensorData = await _sensorDataRepository.GetByIdAsync(command.Id);
        if (existingSensorData == null)
            throw new ArgumentException($"SensorData with ID {command.Id} does not exist.", nameof(command));

        if (command.DeviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(command));

        var device = await _deviceRepository.GetByIdAsync(command.DeviceId);
        if (device == null)
            throw new ArgumentException($"Device with ID {command.DeviceId} does not exist.", nameof(command));

        _mapper.Map(command, existingSensorData);
        var updatedSensorData = await _sensorDataRepository.UpdateAsync(existingSensorData);
        
        return _mapper.Map<SensorDataDto>(updatedSensorData);
    }
}
