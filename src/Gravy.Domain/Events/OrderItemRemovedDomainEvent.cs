namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when an item is reoed to an order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order to which the item was updated.</param>
/// <param name="OrderItemId">Identifier of the updated order item.</param>
public sealed record OrderItemRemovedDomainEvent(
    Guid Id,
    Guid OrderId,
    Guid OrderItemId
) : DomainEvent(Id);
