namespace Gravy.Presentation.Contracts.Restaurants;

public sealed record UpdateRestaurantRequest(
    string Name,
    string Description,
    string Email,
    string PhoneNumber,
    string Address);