using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.MenuItems.UpdateMenuItem;

public sealed class UpdateMenuItemCommandHandler(
    IRestaurantRepository restaurantRepository,
    IMenuItemRepository menuItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateMenuItemCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IMenuItemRepository _menuItemRepository = menuItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var (restaurantId, menuItemId, name, description, price, 
            category, isAvailable) = request;

        #region Get Restaurant 
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId,
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }
        #endregion

        #region Update Menu Item in Restaurant
        Result<MenuItem> updatedMenuItemResult = restaurant.UpdateMenuItem(
            menuItemId,
            name,
            description,
            price,
            category,
            isAvailable);
        if (updatedMenuItemResult.IsFailure)
        {
            return Result.Failure(
                updatedMenuItemResult.Error);
        }
        #endregion

        #region Update database
        _menuItemRepository.Update(updatedMenuItemResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}