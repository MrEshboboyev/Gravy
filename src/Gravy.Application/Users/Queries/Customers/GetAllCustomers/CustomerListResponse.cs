using Gravy.Application.Users.Queries.GetUserById;

namespace Gravy.Application.Users.Queries.Customers.GetAllCustomers;

public sealed record CustomerListResponse(
    IReadOnlyCollection<CustomerDetailsResponse> Customers);

