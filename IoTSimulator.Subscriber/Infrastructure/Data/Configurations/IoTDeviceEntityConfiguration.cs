using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IoTSimulator.Subscriber.Domain.Models;

namespace IoTSimulator.Subscriber.Infrastructure.Data.Configurations;

public class IoTDeviceEntityConfiguration : BaseEntityConfiguration<IoTDevice>
{
    protected override void ConfigureEntity(EntityTypeBuilder<IoTDevice> builder)
    {
        // Properties
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.RoomId);

        builder.Property(d => d.DeviceType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.Manufacturer)
            .HasMaxLength(100);

        builder.Property(d => d.Model)
            .HasMaxLength(100);

        builder.Property(d => d.SerialNumber)
            .HasMaxLength(100);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(d => d.RoomId)
            .HasDatabaseName("IX_IoTDevices_RoomId");

        builder.HasIndex(d => d.DeviceType)
            .HasDatabaseName("IX_IoTDevices_DeviceType");

        builder.HasIndex(d => d.IsActive)
            .HasDatabaseName("IX_IoTDevices_IsActive");

        builder.HasIndex(d => d.SerialNumber)
            .IsUnique()
            .HasFilter("\"SerialNumber\" IS NOT NULL")
            .HasDatabaseName("IX_IoTDevices_SerialNumber");

        // Relationships
        builder.HasOne(d => d.Room)
            .WithMany(r => r.Devices)
            .HasForeignKey(d => d.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.SensorDataList)
            .WithOne(s => s.Device)
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
