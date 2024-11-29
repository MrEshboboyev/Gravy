
namespace Gravy.Presentation.Contracts.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    Guid RestaurantId,
    string Street,
    string City,
    string State,
    string PostalCode);
