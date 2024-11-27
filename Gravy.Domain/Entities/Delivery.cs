using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents delivery details for an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class Delivery : IAuditableEntity
{
    // Constructor
    private Delivery(Guid id, Guid orderId, Guid deliveryPersonId, 
        TimeSpan estimatedDeliveryTime) 
    {
        Id = id;
        OrderId = orderId;
        DeliveryPersonId = deliveryPersonId;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        DeliveryStatus = DeliveryStatus.Assigned;
    }

    private Delivery()
    {
    }

    // Properties
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }
    public DateTime? PickUpTime { get; private set; }
    public TimeSpan EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a delivery.
    /// </summary>
    public static Delivery Create(Guid id, 
        Guid orderId, 
        Guid deliveryPersonId, 
        TimeSpan estimatedDeliveryTime)
    {
        return new Delivery(id, orderId, deliveryPersonId, estimatedDeliveryTime);
    }

    /// <summary>
    /// Marks the delivery as completed.
    /// </summary>
    public void MarkAsDelivered()
    {
        DeliveryStatus = DeliveryStatus.Delivered;
        ActualDeliveryTime = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}

