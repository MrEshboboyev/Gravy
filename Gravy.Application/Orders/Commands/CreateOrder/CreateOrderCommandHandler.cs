using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Orders.Commands.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IRestaurantRepository restaurantRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, 
        CancellationToken cancellationToken)
    {
        var (customerId, restaurantId, street, city, state, latitude, longitude)
            = request;

        // checking restaurant exists
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, 
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure<Guid>(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }

        #region Prepare ValueObjects
        Result<DeliveryAddress> deliveryAddressResult = DeliveryAddress.Create(
            street, 
            city, 
            state, 
            latitude, 
            longitude);
        #endregion

        var order = Order.Create(
            Guid.NewGuid(),
            customerId,
            restaurantId,
            deliveryAddressResult.Value);

        _orderRepository.Add(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}