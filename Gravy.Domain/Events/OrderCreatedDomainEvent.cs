namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new order is created.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the created order.</param>
/// <param name="CustomerId">Identifier of the customer who placed the order.</param>
/// <param name="PlacedAt">The timestamp when the order was placed.</param>
public sealed record OrderCreatedDomainEvent(Guid Id, Guid OrderId, Guid CustomerId, DateTime PlacedAt)
    : DomainEvent(Id);
