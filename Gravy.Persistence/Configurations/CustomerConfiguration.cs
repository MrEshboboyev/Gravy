using Gravy.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gravy.Persistence.Configurations;

/// <summary>
/// Configures the Customer entity for Entity Framework Core.
/// </summary>
internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Map to the Customers table
        builder.ToTable(TableNames.Customers);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure the DefaultDeliveryAddress as an owned type
        builder.OwnsOne(x => x.DefaultDeliveryAddress, addressBuilder =>
        {
            addressBuilder.Property(x => x.Street)
                .HasColumnName("Street")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(x => x.City)
                .HasColumnName("City")
                .HasMaxLength(50)
                .IsRequired();

            addressBuilder.Property(x => x.State)
                .HasColumnName("State")
                .HasMaxLength(50)
                .IsRequired();

            addressBuilder.Property(x => x.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Configure FavoriteRestaurants with value converter and comparer
        builder.Property(x => x.FavoriteRestaurants)
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
