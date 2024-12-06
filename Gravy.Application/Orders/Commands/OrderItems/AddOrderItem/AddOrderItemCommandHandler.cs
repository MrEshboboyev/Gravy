using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

internal sealed class AddOrderItemCommandHandler(IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddOrderItemCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var (orderId, menuItemId, quantity, price) = request;

        // checking order exists
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        order.AddOrderItem(
            menuItemId,
            quantity,
            price);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

