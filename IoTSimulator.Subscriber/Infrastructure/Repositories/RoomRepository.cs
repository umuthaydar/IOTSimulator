using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Data;

namespace IoTSimulator.Subscriber.Infrastructure.Repositories;

public class RoomRepository : Repository<Room>
{

    public RoomRepository(IoTDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Room>> GetByHouseIdAsync(Guid houseId)
    {
        return await _context.Rooms
            .Where(r => r.HouseId == houseId)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Room?> GetWithDevicesAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.Devices)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room?> GetWithCompleteHierarchyAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.House)
            .Include(r => r.Devices)
                .ThenInclude(d => d.SensorDataList.OrderByDescending(s => s.Timestamp).Take(10))
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Room>> GetByHouseIdAndNameAsync(Guid houseId, string nameFilter)
    {
        var query = _context.Rooms.Where(r => r.HouseId == houseId);

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(r => r.Name.Contains(nameFilter));
        }

        return await query.OrderBy(r => r.Name).ToListAsync();
    }

    // House ile birlikte tüm room'ları getir
    public async Task<IEnumerable<Room>> GetAllWithHouseAsync()
    {
        return await _context.Rooms
            .Include(r => r.House)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
}
