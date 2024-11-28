using Gravy.Domain.Enums;

namespace Gravy.Application.Orders.Queries.GetOrderById;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    Guid RestaurantId,
    string DeliveryAddress,
    OrderStatus OrderStatus,
    DateTime PlacedAt,
    DateTime? DeliveredAt,
    DateTime CreatedOnUtc,
    IReadOnlyCollection<OrderItemResponse> OrderItemDetails,
    DeliveryResponse DeliveryDetails,
    PaymentResponse PaymentDetails);
