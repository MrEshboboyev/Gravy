using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderResponse>;

