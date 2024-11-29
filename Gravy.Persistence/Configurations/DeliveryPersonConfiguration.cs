using Gravy.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gravy.Persistence.Configurations;

/// <summary>
/// Configures the DeliveryPerson entity for Entity Framework Core.
/// </summary>
internal sealed class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
{
    public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
    {
        // Map to the DeliveryPersons table
        builder.ToTable(TableNames.DeliveryPersons);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure the Vehicle as an owned type
        builder.OwnsOne(x => x.Vehicle, vehicleBuilder =>
        {
            vehicleBuilder.Property(x => x.Type)
                .HasColumnName("VehicleType")
                .HasMaxLength(50)
                .IsRequired();

            vehicleBuilder.Property(x => x.LicensePlate)
                .HasColumnName("LicensePlate")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Configure AssignedDeliveries with value converter and comparer
        builder.Property(x => x.AssignedDeliveries)
            .HasConversion(
                x => string.Join(",", x),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .Metadata.SetValueComparer(new ValueComparer<ICollection<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2), // Compare collections by their sequence
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Compute hash
                c => c.ToList())); // Snapshot for tracking

        // Add audit properties
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);
    }
}
