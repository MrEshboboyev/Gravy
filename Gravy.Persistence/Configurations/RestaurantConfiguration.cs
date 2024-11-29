using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;

namespace Gravy.Persistence.Configurations;

/// <summary>
/// Configuration for the Restaurant entity.
/// </summary>
public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        // Table mapping
        builder.ToTable("Restaurants");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100); // Example max length

        builder.Property(r => r.Description)
            .HasMaxLength(500); // Example max length

        builder.Property(r => r.Email)
            .IsRequired()
            .HasConversion(
                email => email.Value, // Convert to string for storage
                value => Email.Create(value).Value); // Convert back to Email

        builder.Property(r => r.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedOnUtc)
            .IsRequired();

        builder.Property(r => r.ModifiedOnUtc);

        // Relationships
        builder.HasMany(r => r.MenuItems)
            .WithOne()
            .HasForeignKey(m => m.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Value Objects
        builder.OwnsOne(r => r.Address, navigation =>
        {
            navigation.Property(a => a.Value).IsRequired().HasMaxLength(200);
        });

        builder.OwnsOne(r => r.OpeningHours, navigation =>
        {
            navigation.Property(o => o.OpenTime).IsRequired();
            navigation.Property(o => o.CloseTime).IsRequired();
        });

        // Indexes
        builder.HasIndex(r => r.Email).IsUnique();
    }
}
