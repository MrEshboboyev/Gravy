using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.Customers.GetAllCustomers;

public sealed record GetAllCustomersQuery : IQuery<CustomerListResponse>;

