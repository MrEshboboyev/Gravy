using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.ActivateRestaurant;

public sealed record ActivateRestaurantCommand(
    Guid RestaurantId) : ICommand;

