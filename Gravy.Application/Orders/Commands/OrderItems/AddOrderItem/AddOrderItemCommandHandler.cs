using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

internal sealed class AddOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IMenuItemRepository menuItemRepository,
    IUnitOfWork unitOfWork,
    IPricingService pricingService) : ICommandHandler<AddOrderItemCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
    private readonly IMenuItemRepository _menuItemRepository = menuItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPricingService _pricingService = pricingService;

    public async Task<Result> Handle(AddOrderItemCommand request, 
        CancellationToken cancellationToken)
    {
        var (orderId, menuItemId, quantity) = request;

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

        #region Get Menu Item by Id

        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId, 
            cancellationToken);
        if (menuItem is null)
        {
            return Result.Failure(
                DomainErrors.MenuItem.NotFound(menuItemId));
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

        #region Add Order Item to this Order

        var addOrderItemResult = order.AddOrderItem(
            menuItem.Id,
            request.Quantity,
            finalPriceResult.Value);
        if (addOrderItemResult.IsFailure)
        {
            return Result.Failure(addOrderItemResult.Error);
        }

        #endregion

        #region Add and Update database

        _orderItemRepository.Add(addOrderItemResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        #endregion

        return Result.Success();
    }
}

