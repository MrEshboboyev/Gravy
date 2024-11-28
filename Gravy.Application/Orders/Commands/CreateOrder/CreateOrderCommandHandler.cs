using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Orders.Commands.CreateOrder;

internal sealed class CreateOrderCommandHandler(IOrderRepository orderRepository, 
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var (customerId, restaurantId, street, city, state, postalCode) = request;

        Result<DeliveryAddress> deliveryAddressResult = DeliveryAddress.Create(street, city, 
            state, postalCode);

        var order = Order.Create(
            Guid.NewGuid(),
            customerId,
            restaurantId,
            deliveryAddressResult.Value);

        _orderRepository.Add(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

