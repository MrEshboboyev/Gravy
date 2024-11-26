namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when an order is marked as delivered.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the delivered order.</param>
/// <param name="DeliveredAt">The timestamp when the order was delivered.</param>
public sealed record OrderDeliveredDomainEvent(Guid Id, Guid OrderId, DateTime DeliveredAt)
    : DomainEvent(Id);
