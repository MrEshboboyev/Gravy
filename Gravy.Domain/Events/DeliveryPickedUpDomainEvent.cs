namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery is picked up by the delivery person.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryId">Identifier of the delivery.</param>
/// <param name="PickUpTime">The timestamp when the delivery was picked up.</param>
public sealed record DeliveryPickedUpDomainEvent(Guid Id, Guid DeliveryId, DateTime PickUpTime)
    : DomainEvent(Id);
