using Gravy.Application.Restaurants.Queries.GetRestaurantById;

namespace Gravy.Application.Restaurants.Queries.GetMenuItemsByCategory;

/// <summary>
/// Represents a list of menu items returned in the search query.
/// </summary>
public sealed record MenuItemListResponse(IReadOnlyCollection<MenuItemResponse> MenuItems);
