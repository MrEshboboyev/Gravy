using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Restaurants.Queries.GetRestaurantById;
using Gravy.Application.Restaurants.Queries.SearchRestaurantsByName;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantOwner;

internal sealed class GetRestaurantOwnerQueryHandler(
    IRestaurantRepository restaurantRepository,
    IUserRepository userRepository) 
    : IQueryHandler<GetRestaurantOwnerQuery, RestaurantOwnerResponse>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<RestaurantOwnerResponse>> Handle(GetRestaurantOwnerQuery request, 
        CancellationToken cancellationToken)
    {
        Restaurant restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId,
            cancellationToken);

        if (restaurant is null)
        {
            return Result.Failure<RestaurantOwnerResponse>(
                DomainErrors.Restaurant.NotFound(request.RestaurantId));
        }

        #region Prepare Response
        var owner = await _userRepository.GetByIdAsync(restaurant.OwnerId, cancellationToken);

        var ownerRestaurants = await _restaurantRepository.GetByOwnerIdAsync(owner.Id, 
            cancellationToken);    

        var restaurantsResponse = new RestaurantListResponse(
            ownerRestaurants.Select(restaurant => new RestaurantResponse(
                restaurant.Id,
                restaurant.IsActive,
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

        var ownerFullName = $"{owner.FirstName.Value} {owner.LastName.Value}";

        var response = new RestaurantOwnerResponse(
            owner.Email.Value,
            ownerFullName,
            restaurantsResponse);
        #endregion

        return response;
    }
}