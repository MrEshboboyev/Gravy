using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.DeleteOrder;

internal sealed class DeleteOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteOrderCommand request,
        CancellationToken cancellationToken)
    {
        var orderId = request.OrderId;

        #region Get Order

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        #endregion

        #region Check Order is Locked

        if (order.IsLocked)
        {
            return Result.Failure(
                DomainErrors.Order.OrderIsLocked);
        }

        #endregion

        #region Remove and Update database

        // Remove this Order
        _orderRepository.Remove(order);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}