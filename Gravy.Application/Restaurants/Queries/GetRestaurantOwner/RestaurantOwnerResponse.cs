using Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantOwner;

public sealed record RestaurantOwnerResponse(
    string Email, 
    string FullName,
    RestaurantListResponse Restaurants);

