using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Payments.CompletePayment;

internal sealed class CompletePaymentCommandHandler(IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CompletePaymentCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CompletePaymentCommand request,
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

        #region Complete Payment for this Order
        order.CompletePayment();
        #endregion

        #region Update database
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(order);
    }
}
