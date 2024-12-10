using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Orders.Queries.Common;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Queries.GetOrdersByCustomer;

internal sealed class GetOrdersByCustomerQueryHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository) : 
    IQueryHandler<GetOrdersByCustomerQuery, OrderListResponse>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<OrderListResponse>> Handle(GetOrdersByCustomerQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        #region Get User with Customer Details
        var user = await _userRepository.GetByIdWithCustomerDetailsAsync(userId,
            cancellationToken);
        if (user is null)
        {
            return Result.Failure<OrderListResponse>(
                DomainErrors.User.NotFound(userId));
        }
        var customer = user.CustomerDetails;
        #endregion

        #region Get Customer Orders By CustomerId
        var orders = await _orderRepository.GetByCustomerIdAsync(user.CustomerDetails.Id, 
            cancellationToken);
        #endregion

        #region Prepare OrderList response
        var response = new OrderListResponse(
            orders
            .Select(OrderResponseFactory.Create)
            .ToList());
        #endregion

        return response;
    }
}
