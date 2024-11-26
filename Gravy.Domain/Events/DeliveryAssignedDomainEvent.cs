namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery person is assigned to a delivery.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryId">Identifier of the delivery.</param>
/// <param name="DeliveryPersonId">Identifier of the assigned delivery person.</param>
/// <param name="AssignedAt">The timestamp when the assignment occurred.</param>
public sealed record DeliveryAssignedDomainEvent(Guid Id, 
    Guid DeliveryId, 
    Guid DeliveryPersonId, 
    DateTime AssignedAt)
    : DomainEvent(Id);
