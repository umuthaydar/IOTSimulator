using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IoTSimulator.Subscriber.Domain.Models;

namespace IoTSimulator.Subscriber.Infrastructure.Data.Configurations;

public class SensorDataEntityConfiguration : BaseEntityConfiguration<SensorData>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SensorData> builder)
    {
        // Properties
        builder.Property(s => s.DeviceId)
            .IsRequired();

        builder.Property(s => s.SensorId)
            .HasMaxLength(50);

        builder.Property(s => s.SensorName)
            .HasMaxLength(200);

        builder.Property(s => s.Location)
            .HasMaxLength(100);

        builder.Property(s => s.Temperature)
            .HasPrecision(5, 2);

        builder.Property(s => s.Humidity)
            .HasPrecision(5, 2);

        builder.Property(s => s.Timestamp)
            .IsRequired();

        // Indexes for better query performance
        builder.HasIndex(s => s.DeviceId)
            .HasDatabaseName("IX_SensorData_DeviceId");

        builder.HasIndex(s => s.Timestamp)
            .HasDatabaseName("IX_SensorData_Timestamp");

        builder.HasIndex(s => s.SensorId)
            .HasDatabaseName("IX_SensorData_SensorId");

        builder.HasIndex(s => new { s.DeviceId, s.Timestamp })
            .HasDatabaseName("IX_SensorData_DeviceId_Timestamp");

        // Relationships
        builder.HasOne(s => s.Device)
            .WithMany(d => d.SensorDataList)
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
