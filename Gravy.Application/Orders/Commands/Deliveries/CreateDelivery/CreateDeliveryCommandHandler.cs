using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Deliveries.CreateDelivery;

internal sealed class CreateDeliveryCommandHandler(
    IOrderRepository orderRepository, 
    IDeliveryRepository deliveryRepository, 
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDeliveryCommand, Guid>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IDeliveryRepository _deliveryRepository = deliveryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateDeliveryCommand request, 
        CancellationToken cancellationToken)
    {
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

        #region Create Delivery for this Order

        var createDeliveryResult = order.CreateDelivery();
        if (createDeliveryResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createDeliveryResult.Error);
        }

        #endregion

        #region Add and Update database
        _deliveryRepository.Add(createDeliveryResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        #endregion

        return Result.Success(createDeliveryResult.Value.Id);
    }
}
