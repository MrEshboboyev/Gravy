using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Deliviries.CompleteDelivery;

internal sealed class CompleteDeliveryCommandHandler(IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CompleteDeliveryCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CompleteDeliveryCommand request, CancellationToken cancellationToken)
    {
        var orderId = request.OrderId;

        // checking order exists
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        order.CompleteDelivery();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(order);
    }
}
