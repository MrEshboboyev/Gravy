﻿using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;

internal sealed class RemoveMenuItemCommandHandler(IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveMenuItemCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveMenuItemCommand request, CancellationToken cancellationToken)
    {
        var (restaurantId, menuItemId) = request;

        // checking restaurant exists
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        // handle result errors 
        var removeResult = restaurant.RemoveMenuItem(menuItemId);
        if (removeResult.IsFailure)
        {
            return Result.Failure(
                removeResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}