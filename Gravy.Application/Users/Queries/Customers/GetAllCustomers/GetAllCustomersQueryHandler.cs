using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Users.Queries.GetUserById;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Queries.Customers.GetAllCustomers;

internal sealed class GetAllCustomersQueryHandler(
    ICustomerRepository customerRepository) : IQueryHandler<GetAllCustomersQuery, CustomerListResponse>
{
    private readonly ICustomerRepository _customerRepository
        = customerRepository;

    public async Task<Result<CustomerListResponse>> Handle(GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var allCustomers = await _customerRepository.GetAllAsync(cancellationToken);

        var response = new CustomerListResponse(
            allCustomers
                .Select(customer => new CustomerDetailsResponse(
                    customer.Id,
                    customer.UserId,
                    PrepareDeliveryAddress(customer.DefaultDeliveryAddress),
                    customer.FavoriteRestaurants,
                    customer.CreatedOnUtc))
                .ToList());

        return response;
    }

    static string PrepareDeliveryAddress(DeliveryAddress deliveryAddress) =>
               $"{deliveryAddress.Street}/" +
               $"{deliveryAddress.City}/" +
               $"{deliveryAddress.State}/" +
               $"{deliveryAddress.PostalCode}";
}

