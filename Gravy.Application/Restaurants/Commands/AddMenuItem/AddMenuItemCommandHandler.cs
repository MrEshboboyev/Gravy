using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Restaurants.Commands.AddMenuItem;

internal sealed class AddMenuItemCommandHandler(IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddMenuItemCommand>
{
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddMenuItemCommand request, CancellationToken cancellationToken)
    {
        var (restaurantId, name, description, price, category) = request;

        // checking restaurant exists
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        restaurant.AddMenuItem(
            Guid.NewGuid(),
            name,
            description,
            price,
            category);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Result.Failure(
                DomainErrors.Restaurant.Concurrency);
        }

        return Result.Success();
    }
}