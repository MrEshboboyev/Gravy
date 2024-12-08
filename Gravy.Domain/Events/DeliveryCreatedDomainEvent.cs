namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when created a delivery in order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order.</param>
/// <param name="DeliveryId">Identifier of the delivery.</param>
/// <param name="CreatedAt">The timestamp when the created a delivery.</param>
public sealed record DeliveryCreatedDomainEvent(Guid Id, 
    Guid OrderId,
    Guid DeliveryId, 
    DateTime CreatedAt)
    : DomainEvent(Id);
