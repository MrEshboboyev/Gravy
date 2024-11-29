namespace Gravy.Presentation.Contracts.Orders;

public sealed record AddOrderItemRequest(
    Guid MenuItemId,
    int Quantity,
    decimal Price);
