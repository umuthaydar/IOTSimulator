using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Data;

namespace IoTSimulator.Subscriber.Infrastructure.Repositories;

public class SensorDataRepository : Repository<SensorData>
{
    
    public SensorDataRepository(IoTDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SensorData>> GetByDeviceIdAsync(Guid deviceId)
    {
        return await _context.SensorData
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorData>> GetByDeviceIdAndDateRangeAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        return await _context.SensorData
            .Where(s => s.DeviceId == deviceId && s.Timestamp >= startDate && s.Timestamp <= endDate)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<SensorData?> GetLatestByDeviceIdAsync(Guid deviceId)
    {
        return await _context.SensorData
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SensorData>> GetBySensorIdAsync(string sensorId)
    {
        if (string.IsNullOrWhiteSpace(sensorId))
            return Enumerable.Empty<SensorData>();

        return await _context.SensorData
            .Where(s => s.SensorId == sensorId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.SensorData
            .Where(s => s.Timestamp >= startDate && s.Timestamp <= endDate)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorData>> GetRecentAsync(int count = 100)
    {
        return await _context.SensorData
            .OrderByDescending(s => s.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorData>> GetWithDeviceInfoAsync(Guid? deviceId = null, int count = 100)
    {
        IQueryable<SensorData> query = _context.SensorData
            .Include(s => s.Device)
                .ThenInclude(d => d.Room != null ? d.Room : null!)
                    .ThenInclude(r => r != null && r.House != null ? r.House : null!);

        if (deviceId.HasValue)
        {
            query = query.Where(s => s.DeviceId == deviceId.Value);
        }

        return await query
            .OrderByDescending(s => s.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> DeleteOldDataAsync(DateTime olderThan)
    {
        var oldRecords = await _context.SensorData
            .Where(s => s.Timestamp < olderThan)
            .ToListAsync();

        if (oldRecords.Any())
        {
            _context.SensorData.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();
        }

        return oldRecords.Count;
    }

    public async Task<(double? avgTemp, double? avgHumidity, int recordCount)> GetAggregatedDataAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        var data = await _context.SensorData
            .Where(s => s.DeviceId == deviceId && s.Timestamp >= startDate && s.Timestamp <= endDate)
            .ToListAsync();

        if (!data.Any())
            return (null, null, 0);

        var temperatureData = data.Where(s => s.Temperature.HasValue).Select(s => s.Temperature!.Value);
        var humidityData = data.Where(s => s.Humidity.HasValue).Select(s => s.Humidity!.Value);

        var avgTemp = temperatureData.Any() ? temperatureData.Average() : (double?)null;
        var avgHumidity = humidityData.Any() ? humidityData.Average() : (double?)null;

        return (avgTemp, avgHumidity, data.Count);
    }

    public async Task<IEnumerable<SensorData>> GetRecentWithDeviceAndRoomAsync(int count = 10)
    {
        return await _context.SensorData
            .Include(s => s.Device)
                .ThenInclude(d => d.Room != null ? d.Room : null!)
                    .ThenInclude(r => r != null && r.House != null ? r.House : null!)
            .OrderByDescending(s => s.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
