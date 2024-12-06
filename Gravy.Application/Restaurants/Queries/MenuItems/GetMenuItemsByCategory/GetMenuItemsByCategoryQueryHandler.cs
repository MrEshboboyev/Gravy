using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Restaurants.Queries.GetRestaurantById;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Queries.MenuItems.GetMenuItemsByCategory;

internal sealed class GetMenuItemsByCategoryQueryHandler(
    IRestaurantRepository restaurantRepository)
    : IQueryHandler<GetMenuItemsByCategoryQuery, MenuItemListResponse>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;

    public async Task<Result<MenuItemListResponse>> Handle(GetMenuItemsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var (restaurantId, category) = request;

        Restaurant restaurant = await _restaurantRepository.GetByIdAsync(restaurantId,
            cancellationToken);

        if (restaurant is null)
        {
            return Result.Failure<MenuItemListResponse>(
                DomainErrors.Restaurant.NotFound(request.RestaurantId));
        }

        var menuItems = restaurant.MenuItems;

        var response = new MenuItemListResponse(
            menuItems
                .Where(menuItem => menuItem.Category == category)
                .Select(menuItem => new MenuItemResponse(
                menuItem.Id,
                menuItem.Name,
                menuItem.Description,
                menuItem.Price,
                menuItem.Category,
                menuItem.IsAvailable,
                menuItem.CreatedOnUtc)
            ).ToList());

        return response;
    }
}

