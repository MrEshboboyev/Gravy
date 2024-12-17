using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.OrderItems.UpdateOrderItem;

internal sealed class UpdateOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IUnitOfWork unitOfWork,
    IPricingService pricingService,
    IMenuItemRepository menuItemRepository) : 
    ICommandHandler<UpdateOrderItemCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPricingService _pricingService = pricingService;
    private readonly IMenuItemRepository _menuItemRepository = menuItemRepository;

    public async Task<Result> Handle(UpdateOrderItemCommand request, 
        CancellationToken cancellationToken)
    {
        var (orderId, orderItemId, quantity) = request;

        #region Get Order by Id

        var order = await _orderRepository.GetByIdAsync(orderId, 
            cancellationToken);
        if (order is null)
        {
            return Result.Failure(
                DomainErrors.Order.NotFound(orderItemId));
        }

        #endregion

        #region Get Order Item from this Order

        var orderItem = order.OrderItems.FirstOrDefault(
            oi => oi.Id.Equals(orderItemId));
        if (orderItem is null)
        {
            return Result.Failure(
                DomainErrors.OrderItem.NotFound(orderItemId));
        }
        
        #endregion

        #region Get Menu Item by Id

        var menuItem = await _menuItemRepository.GetByIdAsync(orderItem.MenuItemId,
            cancellationToken);
        if (menuItem is null)
        {
            return Result.Failure(
                DomainErrors.MenuItem.NotFound(orderItem.MenuItemId));
        }
        
        #endregion

        #region Calculate Final Price

        var finalPriceResult = _pricingService.CalculatePrice(
            menuItem.Price,
            request.Quantity);
        if (finalPriceResult.IsFailure)
        {
            return Result.Failure(
                finalPriceResult.Error);
        }

        #endregion

        #region Update Order Item details via aggregate root (Order)
        
        var updateOrderItemResult = order.UpdateOrderItem(
            orderItemId,
            quantity,
            finalPriceResult.Value);
        if (updateOrderItemResult.IsFailure)
        {
            return Result.Failure(
                updateOrderItemResult.Error);
        }

        #endregion

        #region Update database

        _orderItemRepository.Update(updateOrderItemResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        #endregion

        return Result.Success();
    }
}