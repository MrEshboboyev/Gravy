using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Restaurants.Queries.GetRestaurantById;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;

/// <summary>
/// Handles the search of restaurants based on the provided name search term.
/// </summary>
internal sealed class SearchRestaurantsByNameQueryHandler(IRestaurantRepository restaurantRepository)
        : IQueryHandler<SearchRestaurantsByNameQuery, RestaurantListResponse>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;

    public async Task<Result<RestaurantListResponse>> Handle(SearchRestaurantsByNameQuery request,
        CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantRepository.SearchByTermAsync(request.SearchTerm, cancellationToken);

        if (restaurants.Count == 0)
        {
            return Result.Failure<RestaurantListResponse>(
                DomainErrors.Restaurant.NoRestaurantsFound(request.SearchTerm));
        }

        var response = new RestaurantListResponse(
            restaurants.Select(restaurant => new RestaurantResponse(
                restaurant.Id,
                restaurant.Name,
                restaurant.Description,
                restaurant.Email.Value,
                restaurant.PhoneNumber,
                restaurant.Address.Value,
                restaurant.OwnerId,
                [restaurant.OpeningHours.OpenTime, restaurant.OpeningHours.CloseTime],
                restaurant
                    .MenuItems
                    .Select(menuItem => new MenuItemResponse(
                        menuItem.Id,
                        menuItem.Name,
                        menuItem.Description,
                        menuItem.Price,
                        menuItem.Category,
                        menuItem.IsAvailable,
                        menuItem.CreatedOnUtc))
                    .ToList()
            )).ToList()
        );

        return response;
    }
}
