using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Entities;

namespace Gravy.Application.Orders.Queries.Common;

public static class OrderResponseFactory
{
    public static OrderResponse Create(Order order)
    {
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
