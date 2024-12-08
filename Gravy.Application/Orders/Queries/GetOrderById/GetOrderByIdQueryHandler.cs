﻿using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Orders.Queries.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery query, 
        CancellationToken cancellationToken)
    {
        Order order = await _orderRepository.GetByIdAsync(query.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<OrderResponse>(
                DomainErrors.Order.NotFound(query.OrderId));
        }

        #region Prepare Response
        var deliveryAddressObject = order.DeliveryAddress;

        string deliveryAddress = 
            $"{deliveryAddressObject.Street}/" +
            $"{deliveryAddressObject.City}/" +
            $"{deliveryAddressObject.State}";

        var orderItemsResponse = order.OrderItems
                .Select(orderItem => new OrderItemResponse(
                    orderItem.Id,
                    orderItem.MenuItemId,
                    orderItem.Quantity,
                    orderItem.Price,
                    orderItem.CreatedOnUtc))
                .ToList();

        var deliveryResponse = order.Delivery is not null ? new DeliveryResponse(
                order.Delivery.Id,
                order.Delivery.DeliveryPersonId,
                order.Delivery.PickUpTime,
                order.Delivery.EstimatedDeliveryTime,
                order.Delivery.ActualDeliveryTime,
                order.Delivery.DeliveryStatus,
                order.Delivery.CreatedOnUtc) : null;

        var paymentResponse = order.Payment is not null ? new PaymentResponse(
                order.Payment.Id,
                order.Payment.Amount,
                order.Payment.Method,
                order.Payment.Status,
                order.Payment.TransactionId,
                order.Payment.CreatedOnUtc) : null;
        #endregion

        var response = new OrderResponse(
            order.Id,
            order.CustomerId,
            order.RestaurantId,
            deliveryAddress,
            order.Status,
            order.PlacedAt,
            order.DeliveredAt,
            order.CreatedOnUtc,
            orderItemsResponse,
            deliveryResponse,
            paymentResponse);

        return response;
    }
}

