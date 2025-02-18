using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Queries.GetRestaurantById;

internal sealed class GetRestaurantByIdQueryHandler(IRestaurantRepository restaurantRepository)
    : IQueryHandler<GetRestaurantByIdQuery, RestaurantResponse>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;

    public async Task<Result<RestaurantResponse>> Handle(GetRestaurantByIdQuery request, 
        CancellationToken cancellationToken)
    {
        Restaurant restaurant = await  _restaurantRepository.GetByIdAsync(request.RestaurantId,
            cancellationToken);

        if (restaurant is null)
        {
            return Result.Failure<RestaurantResponse>(
                DomainErrors.Restaurant.NotFound(request.RestaurantId));
        }

        var response = new RestaurantResponse(
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
            );

        return response;
    }
}

