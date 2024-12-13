using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Payments.SetPayment;

internal sealed class SetPaymentCommandHandler(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IOrderPricingService orderPricingService,
    IUnitOfWork unitOfWork) : ICommandHandler<SetPaymentCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IOrderPricingService _orderPricingService = orderPricingService;

    public async Task<Result> Handle(SetPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var (orderId, method, transactionId) = request;

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

        #region Calculate Order amount

        var amountResult = await _orderPricingService.CalculateOrderTotalAsync(orderId,
            cancellationToken);
        if (amountResult.IsFailure)
        {
            return Result.Failure(
                amountResult.Error);
        }

        #endregion

        #region Set payment in the order
        var paymentResult = order.SetPayment(
            amountResult.Value, 
            method, 
            transactionId);
        if (paymentResult.IsFailure)
        {
            return Result.Failure(
                paymentResult.Error);
        }
        #endregion

        #region Add and Update database
        // Persist payment entity to repository
        _paymentRepository.Add(paymentResult.Value);

        // update Order root for updating fields
        _orderRepository.Update(order);

        // Save changes atomically
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success();
    }
}
