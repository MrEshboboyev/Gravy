using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Entities;

namespace Gravy.Application.Orders.Queries.Common;

public static class OrderItemResponseFactory
{
    public static OrderItemResponse Create(OrderItem orderItem)
    {
        return new OrderItemResponse(
            orderItem.Id,
            orderItem.MenuItemId,
            orderItem.Quantity,
            orderItem.Price,
            orderItem.CreatedOnUtc);
    }
}