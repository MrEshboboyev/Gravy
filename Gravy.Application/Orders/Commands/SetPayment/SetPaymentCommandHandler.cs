using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.SetPayment;

internal sealed class SetPaymentCommandHandler(IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetPaymentCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(SetPaymentCommand request, CancellationToken cancellationToken)
    {
        var (orderId, amount, method, transactionId) = request;

        // checking order exists
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderId));
        }

        order.SetPayment(
            Guid.NewGuid(), 
            amount, 
            method, 
            transactionId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

