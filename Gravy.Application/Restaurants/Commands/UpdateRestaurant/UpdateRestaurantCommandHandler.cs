﻿using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.UpdateRestaurant;

internal sealed class UpdateRestaurantCommandHandler(IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var (restaurantId, name, description, email, phoneNumber, address) = request;

        // checking restaurant exists
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        Result<Email> emailResult = Email.Create(email);
        Result<Address> addressResult = Address.Create(address);

        restaurant.UpdateDetails(
            name, 
            description, 
            emailResult.Value,
            phoneNumber,
            addressResult.Value);

        _restaurantRepository.Update(restaurant);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
