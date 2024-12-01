using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.UpdateMenuItem;

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
        var (restaurantId, menuItemId, name, description, price, category, isAvailable) = request;

        // Fetch the restaurant aggregate
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(DomainErrors.Restaurant.NotFound(restaurantId));
        }

        // Update the menu item via the aggregate root
        Result<MenuItem> updatedMenuItem = restaurant.UpdateMenuItem(
            menuItemId, 
            name, 
            description, 
            price, 
            category, 
            isAvailable);
        if (updatedMenuItem.IsFailure)
        {
            return Result.Failure(
                updatedMenuItem.Error);
        }

        _menuItemRepository.Update(updatedMenuItem.Value);

        // Save changes via Unit of Work
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}