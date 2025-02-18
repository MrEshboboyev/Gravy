using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;

internal sealed class RemoveMenuItemCommandHandler(
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveMenuItemCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveMenuItemCommand request, CancellationToken cancellationToken)
    {
        var (restaurantId, menuItemId) = request;

        #region Get Restaurant 

        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId,
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        #endregion

        #region Remove Menu Item from this Restaurant

        var removeMenuItemResult = restaurant.RemoveMenuItem(menuItemId);
        if (removeMenuItemResult.IsFailure)
        {
            return Result.Failure(
                removeMenuItemResult.Error);
        }

        #endregion

        #region Update database

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        #endregion

        return Result.Success();
    }
}