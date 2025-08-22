using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IoTSimulator.Subscriber.Domain.Models;

namespace IoTSimulator.Subscriber.Infrastructure.Data.Configurations;

public class RoomEntityConfiguration : BaseEntityConfiguration<Room>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Room> builder)
    {
        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.HouseId)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.HouseId)
            .HasDatabaseName("IX_Rooms_HouseId");

        builder.HasIndex(r => new { r.HouseId, r.Name })
            .IsUnique()
            .HasDatabaseName("IX_Rooms_HouseId_Name");

        // Relationships
        builder.HasOne(r => r.House)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Devices)
            .WithOne(d => d.Room)
            .HasForeignKey(d => d.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
