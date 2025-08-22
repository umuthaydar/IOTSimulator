using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IoTSimulator.Subscriber.Domain.Models;

namespace IoTSimulator.Subscriber.Infrastructure.Data.Configurations;

public class HouseEntityConfiguration : BaseEntityConfiguration<House>
{
    protected override void ConfigureEntity(EntityTypeBuilder<House> builder)
    {
        // Properties
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.Address)
            .IsRequired()
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(h => h.Name)
            .HasDatabaseName("IX_Houses_Name");

        // Relationships
        builder.HasMany(h => h.Rooms)
            .WithOne(r => r.House)
            .HasForeignKey(r => r.HouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
