using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.DeactivateRestaurant;

internal sealed class DeactivateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeactivateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository =
        restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeactivateRestaurantCommand request, 
        CancellationToken cancellationToken)
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

        #region Deactivate Restaurant
        restaurant.Deactivate();
        #endregion

        #region Update database
        _restaurantRepository.Update(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}
