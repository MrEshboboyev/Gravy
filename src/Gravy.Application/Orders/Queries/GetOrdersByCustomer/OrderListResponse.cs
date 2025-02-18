using Gravy.Application.Orders.Queries.GetOrderById;

namespace Gravy.Application.Orders.Queries.GetOrdersByCustomer;

public sealed record OrderListResponse(IReadOnlyCollection<OrderResponse> Orders);

