﻿using Gravy.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Entities;

namespace Gravy.Persistence.Configurations;

/// <summary>
/// Configures the Delivery entity for Entity Framework Core.
/// </summary>
internal sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        // Map to the Deliveries table
        builder.ToTable(TableNames.Deliveries);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure relationships
        builder
            .HasOne<Order>()
            .WithOne(x => x.Delivery)
            .HasForeignKey<Delivery>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure properties
        builder.Property(x => x.DeliveryPersonId)
            .IsRequired();

        builder.Property(x => x.PickUpTime);

        builder.Property(x => x.EstimatedDeliveryTime)
            .IsRequired();

        builder.Property(x => x.ActualDeliveryTime);

        builder.Property(x => x.DeliveryStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc);
    }
}