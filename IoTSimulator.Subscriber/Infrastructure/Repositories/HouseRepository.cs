using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Data;

namespace IoTSimulator.Subscriber.Infrastructure.Repositories;

public class HouseRepository : Repository<House>
{

    public HouseRepository(IoTDbContext context) : base(context)
    {
    }

    public async Task<House?> GetWithRoomsAsync(Guid id)
    {
        return await _context.Houses
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<House?> GetWithCompleteHierarchyAsync(Guid id)
    {
        return await _context.Houses
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Devices)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<House>> GetByNameAsync(string nameFilter)
    {
        if (string.IsNullOrWhiteSpace(nameFilter))
            return await GetAllAsync();

        return await _context.Houses
            .Where(h => h.Name.Contains(nameFilter))
            .OrderBy(h => h.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<House>> GetAllWithRoomsAndDevicesAsync()
    {
        return await _context.Houses
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Devices)
                    .ThenInclude(d => d.SensorDataList)
            .OrderBy(h => h.Name)
            .ToListAsync();
    }
}
