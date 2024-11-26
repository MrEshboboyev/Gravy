using Gravy.Domain.Enums;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer's order in the system.
/// </summary>
public sealed class Order : AggregateRoot, IAuditableEntity
{
    private Order(Guid id, 
        Guid customerId, Guid restaurantId, Guid? deliveryId, 
        decimal totalPrice, 
        DeliveryAddress deliveryAddress, Status status, DateTime placedAt, DateTime? deliveredAt) : base(id)
    {
        CustomerId = customerId;
        RestaurantId = restaurantId;
        DeliveryId = deliveryId;
        TotalPrice = totalPrice;
        DeliveryAddress = deliveryAddress;
        Status = status;
        PlacedAt = placedAt;
        DeliveredAt = deliveredAt;
    }

    private Order()
    {
    }

    // Properties
    public Guid CustomerId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public Guid? DeliveryId { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DeliveryAddress DeliveryAddress { get; private set; }
    public Status Status { get; private set; }
    public DateTime PlacedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new order.
    /// </summary>
    public static Order Create(
        Guid id,
        Guid customerId, 
        Guid restaurantId, 
        Guid? deliveryId, 
        decimal totalPrice,
        DeliveryAddress deliveryAddress
        )
    {
        var order = new Order(
            id,
            customerId,
            restaurantId,
            deliveryId,
            totalPrice,
            deliveryAddress,
            Status.Pending, // Default status
            DateTime.UtcNow,
            null);

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(
            Guid.NewGuid(),
            order.Id,
            order.CustomerId,
            DateTime.UtcNow));

        return order;
    }

    /// <summary>
    /// Updates the delivery status of the order.
    /// </summary>
    public void UpdateStatus(Status newStatus)
    {
        Status = newStatus;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new OrderStatusChangedDomainEvent(
            Guid.NewGuid(),
            this.Id,
            this.Status,
            newStatus,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the order as delivered.
    /// </summary>
    public void MarkAsDelivered()
    {
        Status = Status.Delivered;
        DeliveredAt = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new OrderDeliveredDomainEvent(
            Guid.NewGuid(),
            this.Id,
            DateTime.UtcNow));
    }
}