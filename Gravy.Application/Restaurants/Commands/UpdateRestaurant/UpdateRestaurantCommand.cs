using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.UpdateRestaurant;

public sealed record UpdateRestaurantCommand(
    Guid RestaurantId,
    string Name,
    string Description,
    string Email,
    string PhoneNumber,
    string Address) : ICommand;
