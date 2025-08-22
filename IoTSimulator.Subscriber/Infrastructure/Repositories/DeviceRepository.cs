using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Data;
using IoTSimulator.Subscriber.Domain.Enums;

namespace IoTSimulator.Subscriber.Infrastructure.Repositories;

public class DeviceRepository : Repository<IoTDevice>
{
    
    public DeviceRepository(IoTDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<IoTDevice>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.IoTDevices
            .Where(d => d.RoomId == roomId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<IoTDevice>> GetByTypeAsync(DeviceType deviceType)
    {
        return await _context.IoTDevices
            .Where(d => d.DeviceType == deviceType)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<IoTDevice>> GetActiveDevicesAsync()
    {
        return await _context.IoTDevices
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IoTDevice?> GetBySerialNumberAsync(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        return await _context.IoTDevices
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber);
    }

    public async Task<IEnumerable<IoTDevice>> GetWithLatestSensorDataAsync(Guid? roomId = null)
    {
        IQueryable<IoTDevice> query = _context.IoTDevices
            .Include(d => d.SensorDataList.OrderByDescending(s => s.Timestamp).Take(1));

        if (roomId.HasValue)
        {
            query = query.Where(d => d.RoomId == roomId.Value);
        }

        return await query.OrderBy(d => d.Name).ToListAsync();
    }

    public async Task<IoTDevice?> FindBySensorIdAsync(string sensorId)
    {
        if (string.IsNullOrWhiteSpace(sensorId))
            return null;

        var deviceBySerial = await _context.IoTDevices
            .Include(d => d.Room)
            .FirstOrDefaultAsync(d => d.SerialNumber == sensorId);
        if (deviceBySerial != null)
            return deviceBySerial;

        var deviceByName = await _context.IoTDevices
            .Include(d => d.Room)
            .FirstOrDefaultAsync(d => d.Name.Contains(sensorId));
        if (deviceByName != null)
            return deviceByName;

        var deviceFromSensorData = await _context.SensorData
            .Include(s => s.Device)
                .ThenInclude(d => d.Room)
            .Where(s => s.SensorId == sensorId)
            .Select(s => s.Device)
            .FirstOrDefaultAsync();

        return deviceFromSensorData;
    }

    public async Task<IoTDevice> CreateDefaultDeviceAsync(string sensorId, string? sensorName, string? location)
    {
        var defaultHouse = await _context.Houses.FirstOrDefaultAsync(h => h.Name == "Default House");
        if (defaultHouse == null)
        {
            defaultHouse = new House
            {
                Id = Guid.NewGuid(),
                Name = "Default House",
                Address = "Auto-generated for unknown devices"
            };
            _context.Houses.Add(defaultHouse);
            await _context.SaveChangesAsync();
        }

        var roomName = string.IsNullOrWhiteSpace(location) ? "Unknown Room" : location;
        var defaultRoom = await _context.Rooms
            .FirstOrDefaultAsync(r => r.HouseId == defaultHouse.Id && r.Name == roomName);
        
        if (defaultRoom == null)
        {
            defaultRoom = new Room
            {
                Id = Guid.NewGuid(),
                Name = roomName,
                HouseId = defaultHouse.Id
            };
            _context.Rooms.Add(defaultRoom);
            await _context.SaveChangesAsync();
        }

        var device = new IoTDevice
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(sensorName) ? $"Device {sensorId}" : sensorName,
            RoomId = defaultRoom.Id,
            DeviceType = DeviceType.CombinedSensor,
            SerialNumber = sensorId,
            Manufacturer = "Auto-generated",
            Model = "Unknown"
        };

        await AddAsync(device);
        
        // Room navigation property'yi load etmek için device'ı yeniden çek
        var deviceWithRoom = await _context.IoTDevices
            .Include(d => d.Room)
            .FirstAsync(d => d.Id == device.Id);
            
        return deviceWithRoom;
    }

    public async Task<IEnumerable<IoTDevice>> GetAllWithRoomAsync()
    {
        return await _context.IoTDevices
            .Include(d => d.Room)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<IoTDevice>> GetByRoomIdWithRoomAsync(Guid roomId)
    {
        return await _context.IoTDevices
            .Include(d => d.Room)
            .Where(d => d.RoomId == roomId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}
