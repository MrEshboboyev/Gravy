using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Entities;

namespace Gravy.Application.Orders.Queries.Common;

public static class OrderResponseFactory
{
    public static OrderResponse Create(Order order)
    {
        var deliveryAddress = order.GetDeliveryAddress();

        var orderItemsResponse = order
                .OrderItems
                .Select(OrderItemResponseFactory.Create)
                .ToList();

        var deliveryResponse = order.Delivery is not null
                ? DeliveryResponseFactory.Create(order.Delivery)
                : null;

        var paymentResponse = order.Payment is not null
                ? PaymentResponseFactory.Create(order.Payment)
                : null;

        return new OrderResponse(
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
    }
}