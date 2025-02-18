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

        #region Prepare value objects

        #region Prepare Email

        Result<Email> createEmailResult = Email.Create(email);
        if (createEmailResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createEmailResult.Error);
        }

        #endregion

        #region Prepare Address

        Result<Address> createAddressResult = Address.Create(address);
        if (createAddressResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createAddressResult.Error);
        }

        #endregion

        #region Prepare Opening Hours

        Result<OpeningHours> createOpeningHoursResult = OpeningHours.Create(
            openingHours[0],
            openingHours[1]);
        if (createOpeningHoursResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createOpeningHoursResult.Error);
        }

        #endregion

        #endregion

        #region Create Restaurant

        var restaurant = Restaurant.Create(
            Guid.NewGuid(),
            name,
            description,
            createEmailResult.Value,
            phoneNumber,
            createAddressResult.Value,
            ownerId,
            createOpeningHoursResult.Value
            );

        #endregion

        #region Add and Update database

        _restaurantRepository.Add(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        #endregion

        return Result.Success(restaurant.Id);
    }
}