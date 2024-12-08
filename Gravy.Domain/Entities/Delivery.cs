using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents delivery details for an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class Delivery : Entity
{
    #region Constructors
    internal Delivery(
        Guid id, 
        Guid orderId) : base(id)
    {
        OrderId = orderId;
        DeliveryStatus = DeliveryStatus.Pending;
    }

    private Delivery()
    {
    }
    #endregion

    #region Properties
    public Guid OrderId { get; private set; }
    public Guid? DeliveryPersonId { get; private set; }
    public DateTime? PickUpTime { get; private set; }
    public TimeSpan EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    #endregion

    #region Own methods
    /// <summary>
    /// Marks the delivery as completed.
    /// </summary>
    public void MarkAsDelivered()
    {
        DeliveryStatus = DeliveryStatus.Delivered;
        ActualDeliveryTime = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;
    }
    #endregion
}

