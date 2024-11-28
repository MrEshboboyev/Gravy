namespace Gravy.Application.Orders.Queries.GetOrderById;

public sealed record OrderItemResponse(
    Guid OrderItemId,
    Guid MenuItemId,
    int Quantity,
    decimal Price,
    DateTime CreatedOnUtc);

