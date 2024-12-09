
namespace Gravy.Presentation.Contracts.Orders;

public sealed record CreateOrderRequest(
    Guid RestaurantId,
    string Street,
    string City,
    string State,
    double Latitude,
    double Longitude);
