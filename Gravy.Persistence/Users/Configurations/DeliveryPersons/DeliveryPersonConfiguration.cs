using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Gravy.Persistence.Users.Constants;

namespace Gravy.Persistence.Users.Configurations.DeliveryPersons;

/// <summary>
/// Configures the DeliveryPerson entity for Entity Framework Core.
/// </summary>
internal sealed class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
{
    public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
    {
        // Map to the DeliveryPersons table
        builder.ToTable(UserTableNames.DeliveryPersons);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure relationships
        builder
            .HasOne<User>()
            .WithOne(u => u.DeliveryPersonDetails)
            .HasForeignKey<DeliveryPerson>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete DeliveryPerson when User is deleted

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

        // Configure Location as an owned type
        builder.OwnsOne(x => x.HomeLocation, locationBuilder =>
        {
            locationBuilder.Property(x => x.Latitude)
                .HasColumnName("HomeLatitude")
                .IsRequired();

            locationBuilder.Property(x => x.Longitude)
                .HasColumnName("HomeLongitude")
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

        // Map the new IsAvailable property
        builder.Property(x => x.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true); // Default value: true

        // Add audit properties
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);

        builder
            .HasMany(d => d.Availabilities)
            .WithOne()
            .HasForeignKey(p => p.DeliveryPersonId);
    }
}
