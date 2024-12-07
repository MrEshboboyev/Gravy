using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;

internal sealed class AssignDeliveryCommandHandler(IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignDeliveryCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AssignDeliveryCommand request, CancellationToken cancellationToken)
    {
        var (orderId, deliveryPersonId, estimatedDeliveryTime) = request;

        // checking order exists
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        order.AssignDelivery(
            deliveryPersonId,
            estimatedDeliveryTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

