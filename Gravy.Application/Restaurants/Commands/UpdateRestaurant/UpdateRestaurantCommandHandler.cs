using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.UpdateRestaurant;

internal sealed class UpdateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = 
        restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var (restaurantId, name, description, 
            email, phoneNumber, address) = request;

        #region Get Restaurant 

        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        #endregion

        #region Prepare value objects

        #region Prepare Email

        Result<Email> createEmailResult = Email.Create(email);
        if (createEmailResult.IsFailure)
        {
            return Result.Failure(
                createEmailResult.Error);
        }
        
        #endregion

        #endregion

        #region Prepare Address

        Result<Address> createAddressResult = Address.Create(address);
        if (createAddressResult.IsFailure)
        {
            return Result.Failure(
                createAddressResult.Error);
        }

        #endregion

        #region Update Restaurant Details

        restaurant.UpdateDetails(
            name, 
            description, 
            createEmailResult.Value,
            phoneNumber,
            createAddressResult.Value);

        #endregion

        #region Update database

        _restaurantRepository.Update(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        #endregion

        return Result.Success();
    }
}

