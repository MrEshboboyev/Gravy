namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery is marked as completed.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryId">Identifier of the delivery.</param>
/// <param name="ActualDeliveryTime">The timestamp when the delivery was completed.</param>
public sealed record DeliveryCompletedDomainEvent(Guid Id, Guid DeliveryId, DateTime ActualDeliveryTime)
    : DomainEvent(Id);
