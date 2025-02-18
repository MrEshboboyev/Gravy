using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.OrderItems.RemoveOrderItem;

internal sealed class RemoveOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveOrderItemCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var (orderId, orderItemId) = request;

        #region Get Order

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        #endregion

        #region Remove order item from this order

        var removeOrderItemResult = order.RemoveOrderItem(orderItemId);
        if (removeOrderItemResult.IsFailure)
        {
            return Result.Failure(
                removeOrderItemResult.Error);
        }

        #endregion

        #region Update Database

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}