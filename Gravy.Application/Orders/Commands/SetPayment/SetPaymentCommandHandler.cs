using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.SetPayment;

internal sealed class SetPaymentCommandHandler(IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetPaymentCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
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

        var paymentResult = order.SetPayment(
            amount, 
            method, 
            transactionId);

        if (paymentResult.IsFailure)
        {
            return Result.Failure(paymentResult.Error);
        }

        _paymentRepository.Add(paymentResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

