using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when an item is added to an order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order to which the item was added.</param>
/// <param name="OrderItemId">Identifier of the added order item.</param>
/// <param name="MenuItemId">Identifier of the menu item added.</param>
/// <param name="Quantity">Quantity of the menu item added.</param>
/// <param name="Price">Price of the menu item added.</param>
public sealed record OrderItemAddedDomainEvent(
    Guid Id,
    Guid OrderId,
    Guid OrderItemId,
    Guid MenuItemId,
    int Quantity,
    decimal Price
) : DomainEvent(Id);
