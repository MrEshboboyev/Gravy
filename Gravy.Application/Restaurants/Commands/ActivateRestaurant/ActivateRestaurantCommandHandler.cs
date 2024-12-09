using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.ActivateRestaurant;

internal sealed class ActivateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = 
        restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ActivateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = request.RestaurantId;

        #region Get Restaurant
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, 
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }
        #endregion

        #region Activate Restaurant
        restaurant.Activate();
        #endregion

        #region Update database
        _restaurantRepository.Update(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}
