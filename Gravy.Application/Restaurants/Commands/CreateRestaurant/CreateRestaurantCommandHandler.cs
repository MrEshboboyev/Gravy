using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.CreateRestaurant;

internal sealed class CreateRestaurantCommandHandler(IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRestaurantCommand, Guid>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var (name, description, email, phoneNumber, address, ownerId, openingHours) = request;

        Result<Email> emailResult = Email.Create(email);
        Result<Address> addressResult = Address.Create(address);
        Result<OpeningHours> openingHoursResult = OpeningHours.Create(openingHours[0], openingHours[1]);

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

        _restaurantRepository.Add(restaurant);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return restaurant.Id;
    }
}

