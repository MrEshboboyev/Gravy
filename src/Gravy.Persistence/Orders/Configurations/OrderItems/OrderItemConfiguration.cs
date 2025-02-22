﻿using Gravy.Domain.Entities;
using Gravy.Persistence.Orders.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gravy.Persistence.Orders.Configurations.OrderItems;

/// <summary>
/// Configures the OrderItem entity for Entity Framework Core.
/// </summary>
internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Map to the OrderItems table
        builder.ToTable(OrderTableNames.OrderItems);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure relationships
        builder
            .HasOne<Order>()
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure properties
        builder.Property(x => x.MenuItemId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc);
    }
}
