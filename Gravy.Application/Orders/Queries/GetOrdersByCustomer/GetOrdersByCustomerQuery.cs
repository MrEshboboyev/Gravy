using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Queries.GetOrdersByCustomer;

public sealed record GetOrdersByCustomerQuery(Guid UserId) 
    : IQuery<OrderListResponse>;

