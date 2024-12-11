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
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IRestaurantRepository _restaurantRepository = restaurantRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, 
        CancellationToken cancellationToken)
    {
        var (userId, restaurantId, street, city, state, 
                latitude, longitude) = request;

        #region Get User with Customer Details
        var user = await _userRepository.GetByIdWithCustomerDetailsAsync(userId,
            cancellationToken);
        if (user is null)
        {
            return Result.Failure<Guid>(
                DomainErrors.User.NotFound(userId));
        }
        var customer = user.CustomerDetails;
        #endregion

        #region Validate this User (penalty, isActive and able to created order)
        var validateCustomerCanPlaceOrderResult = ValidateCustomerCanPlaceOrder(
            customer);
        if (validateCustomerCanPlaceOrderResult.IsFailure)
        {
            return Result.Failure<Guid>(
                validateCustomerCanPlaceOrderResult.Error);
        }
        #endregion

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
        if (deliveryAddressResult.IsFailure)
        {
            return Result.Failure<Guid>(
                deliveryAddressResult.Error);
        }
        #endregion

        #region Create Order
        var order = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            restaurantId,
            deliveryAddressResult.Value);
        #endregion

        #region Add and Update database
        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(order.Id);
    }

    private static Result ValidateCustomerCanPlaceOrder(Customer customer)
    {
        // all validations for creating this customer

        return Result.Success();
    }
}