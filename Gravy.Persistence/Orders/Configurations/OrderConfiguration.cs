using Gravy.Domain.Entities;
using Gravy.Persistence.Orders.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gravy.Persistence.Orders.Configurations;

/// <summary>
/// Configures the Order entity for Entity Framework Core.
/// </summary>
internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Map to the Orders table
        builder.ToTable(OrderTableNames.Orders);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure relationships
        builder
            .HasMany(x => x.OrderItems)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Delivery)
            .WithOne()
            .HasForeignKey<Delivery>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Payment)
            .WithOne()
            .HasForeignKey<Payment>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure properties
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.PlacedAt)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc);

        builder.Property(x => x.DeliveredAt);

        // Configure value objects (DeliveryAddress)
        builder.OwnsOne(x => x.DeliveryAddress, address =>
        {
            address.Property(a => a.Street).IsRequired().HasMaxLength(100);
            address.Property(a => a.City).IsRequired().HasMaxLength(50);
            address.Property(a => a.State).IsRequired().HasMaxLength(50);
            address.Property(a => a.PostalCode).IsRequired().HasMaxLength(10);
        });
    }
}
