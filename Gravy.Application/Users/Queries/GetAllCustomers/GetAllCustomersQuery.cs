using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery : IQuery<CustomerListResponse>;

