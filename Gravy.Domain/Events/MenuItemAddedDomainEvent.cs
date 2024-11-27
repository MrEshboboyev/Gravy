using Gravy.Domain.Enums;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new menu item is added to a restaurant's menu.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the restaurant to which the menu item was added.</param>
/// <param name="MenuItemId">Identifier of the newly added menu item.</param>
/// <param name="Name">Name of the menu item.</param>
/// <param name="Price">Price of the menu item.</param>
/// <param name="Category">Category of the menu item.</param>
public sealed record MenuItemAddedDomainEvent(
    Guid Id,
    Guid RestaurantId,
    Guid MenuItemId,
    string Name,
    decimal Price,
    Category Category
) : DomainEvent(Id);
