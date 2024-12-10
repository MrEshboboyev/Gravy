using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Orders.Queries.Common;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Queries.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery query, 
        CancellationToken cancellationToken)
    {
        #region Get Order By Id
        Order order = await _orderRepository.GetByIdAsync(query.OrderId,
            cancellationToken);
        if (order is null)
        {
            return Result.Failure<OrderResponse>(
                DomainErrors.Order.NotFound(query.OrderId));
        }
        #endregion

        #region Prepare OrderResponse
        var response = OrderResponseFactory.Create(order);
        #endregion

        return response;
    }
}

