using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.DeactivateRestaurant;

public sealed record DeactivateRestaurantCommand(
    Guid RestaurantId) : ICommand;

