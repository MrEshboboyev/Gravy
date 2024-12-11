using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Services.Orders;

public class OrderPricingService(
    IOrderRepository orderRepository) : IOrderPricingService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<Result<decimal>> CalculateOrderTotalAsync(Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, 
            cancellationToken);
        if (order is null)
        {
            return Result.Failure<decimal>(
                DomainErrors.Order.NotFound(orderId));
        }

        // Calculate total amount by summing up the prices of the order items
        return order.OrderItems.Sum(item => item.Price);
    }
}