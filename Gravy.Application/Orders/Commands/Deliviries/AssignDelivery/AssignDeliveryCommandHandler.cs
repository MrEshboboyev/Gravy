using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Services.Deliveries;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.Deliviries.AssignDelivery;

internal sealed class AssignDeliveryCommandHandler(
    IOrderRepository orderRepository, 
    IDeliveryPersonSelector deliveryPersonSelector, 
    IUnitOfWork unitOfWork)
    : ICommandHandler<AssignDeliveryCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IDeliveryPersonSelector _deliveryPersonSelector =
        deliveryPersonSelector;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AssignDeliveryCommand request, 
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, 
            cancellationToken);

        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(request.OrderId));
        }

        #region Select Delivery Person
        var deliveryPerson = await _deliveryPersonSelector
                .SelectBestDeliveryPersonAsync(
                    order,
                    cancellationToken);
        if (deliveryPerson is null)
        {
            return Result.Failure(
                DomainErrors.Delivery.NoAvailableDeliveryPerson);
        }
        #endregion

        #region Determine Estimated Delivery Time
        // fix this estimated time coming soon
        var estimatedDeliveryTime = TimeSpan.FromHours(1);
        #endregion

        #region Assign Delivery
        var assignResult = order.AssignDelivery(deliveryPerson.Id, 
            estimatedDeliveryTime);
        if (assignResult.IsFailure)
        {
            return Result.Failure(assignResult.Error);
        }
        #endregion

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
