using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a menu item is removed from a restaurant's menu.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the restaurant from which the menu item was removed.</param>
/// <param name="MenuItemId">Identifier of the removed menu item.</param>
public sealed record MenuItemRemovedDomainEvent(
    Guid Id,
    Guid RestaurantId,
    Guid MenuItemId
) : DomainEvent(Id);
