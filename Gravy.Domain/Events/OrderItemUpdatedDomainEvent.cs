﻿using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when an item is updated to an order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order to which the item was updated.</param>
/// <param name="OrderItemId">Identifier of the updated order item.</param>
/// <param name="Quantity">Quantity of the menu item updated.</param>
/// <param name="Price">Price of the menu item updated.</param>
public sealed record OrderItemUpdatedDomainEvent(
    Guid Id,
    Guid OrderId,
    Guid OrderItemId,
    int Quantity,
    decimal Price
) : DomainEvent(Id);