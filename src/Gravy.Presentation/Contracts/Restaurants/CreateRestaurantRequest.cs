namespace Gravy.Presentation.Contracts.Restaurants;

public sealed record CreateRestaurantRequest(
    string Name,
    string Description,
    string Email,
    string PhoneNumber,
    string Address,
    TimeSpan[] OpeningHours);