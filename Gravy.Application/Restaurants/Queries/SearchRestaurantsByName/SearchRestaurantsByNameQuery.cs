using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;

/// <summary>
/// Query to search for restaurants by their name.
/// </summary>
public sealed record SearchRestaurantsByNameQuery(string SearchTerm) : IQuery<RestaurantListResponse>;
