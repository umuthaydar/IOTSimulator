using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Domain.Common;

namespace IoTSimulator.Subscriber.Infrastructure.Data;

public class IoTDbContext : DbContext
{
    public IoTDbContext(DbContextOptions<IoTDbContext> options) : base(options)
    {
    }

    public DbSet<House> Houses { get; set; } = null!;

    public DbSet<Room> Rooms { get; set; } = null!;

    public DbSet<IoTDevice> IoTDevices { get; set; } = null!;

    public DbSet<SensorData> SensorData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IoTDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    baseEntity.CreatedAt = DateTime.UtcNow;
                }
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is SensorData sensorData && entry.State == EntityState.Added)
            {
                sensorData.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}
