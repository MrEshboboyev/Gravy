using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Payments.SetPayment;

internal sealed class SetPaymentCommandHandler(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetPaymentCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(SetPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var (orderId, amount, method, transactionId) = request;

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

        #region Set payment in the order
        var paymentResult = order.SetPayment(
            amount, 
            method, 
            transactionId);
        if (paymentResult.IsFailure)
        {
            return Result.Failure(paymentResult.Error);
        }
        #endregion

        #region Add and Update database
        // Persist payment entity to repository
        _paymentRepository.Add(paymentResult.Value);

        // Save changes atomically
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}
