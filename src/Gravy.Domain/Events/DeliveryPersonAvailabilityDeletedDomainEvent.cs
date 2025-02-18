namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when delete delivery person availability.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="DeliveryPersonId">Identifier of the delivery person</param>
/// <param name="DeliveryPersonAvailabilityId">Identifier of the delivery person availability.</param>
public sealed record DeliveryPersonAvailabilityDeletedDomainEvent(
    Guid Id,
    Guid DeliveryPersonId,
    Guid DeliveryPersonAvailabilityId
) : DomainEvent(Id);
