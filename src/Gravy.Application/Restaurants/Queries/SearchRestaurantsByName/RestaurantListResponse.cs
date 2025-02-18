using Gravy.Application.Restaurants.Queries.GetRestaurantById;

namespace Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;

/// <summary>
/// Represents a list of restaurants returned in the search query.
/// </summary>
public sealed record RestaurantListResponse(IReadOnlyCollection<RestaurantResponse> Restaurants);
