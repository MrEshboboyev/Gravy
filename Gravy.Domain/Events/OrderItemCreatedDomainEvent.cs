namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new order item is added to an order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderItemId">Identifier of the created order item.</param>
/// <param name="OrderId">Identifier of the order to which the item belongs.</param>
/// <param name="MenuItemId">Identifier of the menu item associated with the order item.</param>
/// <param name="Quantity">The quantity of the menu item in the order item.</param>
/// <param name="CreatedAt">The timestamp when the order item was created.</param>
public sealed record OrderItemCreatedDomainEvent(Guid Id, Guid OrderItemId, Guid OrderId, Guid MenuItemId, int Quantity, DateTime CreatedAt)
    : DomainEvent(Id);
