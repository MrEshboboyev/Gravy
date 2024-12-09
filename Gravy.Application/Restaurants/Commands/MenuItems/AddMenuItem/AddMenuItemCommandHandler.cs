using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.MenuItems.AddMenuItem;

internal sealed class AddMenuItemCommandHandler(IRestaurantRepository restaurantRepository,
    IMenuItemRepository menuItemRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddMenuItemCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IMenuItemRepository _menuItemRepository = menuItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddMenuItemCommand request, CancellationToken cancellationToken)
    {
        var (restaurantId, name, description, price, category) = request;

        #region Get Restaurant 
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, 
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }
        #endregion

        #region Add Menu Item to this Restaurant
        Result<MenuItem> addMenuItemResult = restaurant.AddMenuItem(
            name,
            description,
            price,
            category);
        if (addMenuItemResult.IsFailure)
        {
            return Result.Failure(
                addMenuItemResult.Error);
        }
        #endregion

        #region Add menu item and Update database
        _menuItemRepository.Add(addMenuItemResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}