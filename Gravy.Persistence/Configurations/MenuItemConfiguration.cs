using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;

namespace Gravy.Persistence.Configurations;

/// <summary>
/// Configuration for the MenuItem entity.
/// </summary>
public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        // Table mapping
        builder.ToTable("MenuItems");

        // Primary key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100); // Example max length

        builder.Property(m => m.Description)
            .HasMaxLength(500); // Example max length

        builder.Property(m => m.Price)
            .IsRequired()
            .HasPrecision(18, 2); // Example precision for monetary values

        builder.Property(m => m.Category)
            .IsRequired()
            .HasConversion(
                category => category.ToString(), // Convert to string for storage
                value => Enum.Parse<Category>(value)); // Convert back to enum

        builder.Property(m => m.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.ModifiedOnUtc);

        // Relationships
        builder.HasOne<Restaurant>()
            .WithMany(r => r.MenuItems)
            .HasForeignKey(m => m.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => new { m.RestaurantId, m.Name }).IsUnique(); // Unique menu item per restaurant
    }
}
