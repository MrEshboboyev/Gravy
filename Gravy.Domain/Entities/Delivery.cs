using Gravy.Domain.Enums;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents the delivery details for an order.
/// </summary>
public sealed class Delivery : AggregateRoot, IAuditableEntity
{
    // Constructor
    private Delivery(Guid id, Guid orderId, Guid deliveryPersonId, 
        TimeSpan estimatedDeliveryTime) : base(id)
    {
        OrderId = orderId;
        DeliveryPersonId = deliveryPersonId;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        DeliveryStatus = DeliveryStatus.Assigned;
    }

    private Delivery()
    {
    }

    // Properties
    public Guid OrderId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }
    public DateTime? PickUpTime { get; private set; }
    public TimeSpan EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new delivery.
    /// </summary>
    public static Delivery Create(
        Guid id,
        Guid orderId, 
        Guid deliveryPersonId, 
        TimeSpan estimatedDeliveryTime)
    {
        var delivery = new Delivery(
            id,
            orderId,
            deliveryPersonId,
            estimatedDeliveryTime);

        return delivery;
    }

    /// <summary>
    /// Assigns a delivery person to the delivery.
    /// </summary>
    public void AssignDeliveryPerson(Guid deliveryPersonId)
    {
        DeliveryPersonId = deliveryPersonId;
        DeliveryStatus = DeliveryStatus.Assigned;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new DeliveryAssignedDomainEvent(
            Guid.NewGuid(),
            this.Id,
            deliveryPersonId,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the delivery as picked up.
    /// </summary>
    public void MarkAsPickedUp()
    {
        DeliveryStatus = DeliveryStatus.PickedUp;
        PickUpTime = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new DeliveryPickedUpDomainEvent(
            Guid.NewGuid(),
            this.Id,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the delivery as completed.
    /// </summary>
    public void MarkAsDelivered()
    {
        DeliveryStatus = DeliveryStatus.Delivered;
        ActualDeliveryTime = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new DeliveryCompletedDomainEvent(
            Guid.NewGuid(),
            this.Id,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the delivery as failed.
    /// </summary>
    public void MarkAsFailed()
    {
        DeliveryStatus = DeliveryStatus.Failed;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new DeliveryFailedDomainEvent(
            Guid.NewGuid(),
            this.Id,
            DateTime.UtcNow));
    }
}

