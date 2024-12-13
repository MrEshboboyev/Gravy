using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Deliveries.CompleteDelivery;

internal sealed class CompleteDeliveryCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CompleteDeliveryCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CompleteDeliveryCommand request, 
        CancellationToken cancellationToken)
    {
        var orderId = request.OrderId;

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

        #region Complete this Delivery
        order.CompleteDelivery();
        #endregion

        #region Update database
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(order);
    }
}
