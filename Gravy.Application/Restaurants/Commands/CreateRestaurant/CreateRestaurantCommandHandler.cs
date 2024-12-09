using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.CreateRestaurant;

internal sealed class CreateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRestaurantCommand, Guid>
{
    private readonly IRestaurantRepository _restaurantRepository = 
        restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var (name, description, email, phoneNumber, address, 
            ownerId, openingHours) = request;

        #region Prepare Email
        Result<Email> emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(
                emailResult.Error);
        }
        #endregion

        #region Prepare Address
        Result<Address> addressResult = Address.Create(address);
        if (addressResult.IsFailure) 
        {
            return Result.Failure<Guid>(
                addressResult.Error);
        }
        #endregion

        #region Prepare Opening Hours
        Result<OpeningHours> openingHoursResult = OpeningHours.Create(
            openingHours[0], 
            openingHours[1]);
        if (openingHoursResult.IsFailure)
        {
            return Result.Failure<Guid>(
                openingHoursResult.Error);  
        }
        #endregion

        #region Create Restaurant
        var restaurant = Restaurant.Create(
            Guid.NewGuid(),
            name,
            description,
            emailResult.Value,
            phoneNumber,
            addressResult.Value,
            ownerId,
            openingHoursResult.Value
            );
        #endregion

        #region Add and Update database
        _restaurantRepository.Add(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(restaurant.Id);
    }
}

