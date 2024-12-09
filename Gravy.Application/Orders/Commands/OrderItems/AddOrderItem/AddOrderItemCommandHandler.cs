using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

internal sealed class AddOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddOrderItemCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddOrderItemCommand request, 
        CancellationToken cancellationToken)
    {
        var (orderId, menuItemId, quantity, price) = request;

        #region Get Order
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);
        if (order is null)
        {
            return Result.Failure<Guid>(
                DomainErrors.Order.NotFound(request.OrderId));
        }
        #endregion

        #region Add Order Item to this Order
        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            quantity,
            price);
        if (addOrderItemResult.IsFailure)
        {
            return Result.Failure(
                addOrderItemResult.Error);
        }
        #endregion

        #region Add and Update database
        _orderItemRepository.Add(addOrderItemResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}

