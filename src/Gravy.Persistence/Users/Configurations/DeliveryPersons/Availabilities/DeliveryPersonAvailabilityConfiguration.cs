using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Gravy.Persistence.Users.Constants;

namespace Gravy.Persistence.Users.Configurations.DeliveryPersons.Availabilities;

/// <summary>
/// Configures the DeliveryPersonAvailability entity for Entity Framework Core.
/// </summary>
internal sealed class DeliveryPersonAvailabilityConfiguration : IEntityTypeConfiguration<DeliveryPersonAvailability>
{
    public void Configure(EntityTypeBuilder<DeliveryPersonAvailability> builder)
    {
        // Map to the DeliveryPersonAvailabilities table
        builder.ToTable(UserTableNames.DeliveryPersonAvailabilities);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeliveryPersonId)
            .IsRequired();

        // Map the foreign key relationship
        builder
            .HasOne<DeliveryPerson>() // Each availability belongs to a DeliveryPerson
            .WithMany(dp => dp.Availabilities) // A delivery person can have multiple availability periods
            .HasForeignKey(x => x.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete availabilities when DeliveryPerson is deleted

        // Map StartTimeUtc and EndTimeUtc
        builder.Property(x => x.StartTimeUtc)
            .IsRequired();

        builder.Property(x => x.EndTimeUtc)
            .IsRequired();
    }
}
