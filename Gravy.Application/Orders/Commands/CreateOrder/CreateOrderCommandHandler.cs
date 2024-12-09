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

        #region Get Restaurant
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, 
            cancellationToken);
        if (restaurant is null)
        {
            return Result.Failure<Guid>(
                DomainErrors.Restaurant.NotFound(restaurantId));
        }
        #endregion

        #region Prepare Delivery Address for this Order
        Result<DeliveryAddress> deliveryAddressResult = DeliveryAddress.Create(
            street, 
            city, 
            state, 
            latitude, 
            longitude);
        #endregion

        #region Create Order
        var order = Order.Create(
            Guid.NewGuid(),
            customerId,
            restaurantId,
            deliveryAddressResult.Value);
        #endregion

        #region Add and Update database
        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(order.Id);
    }
}