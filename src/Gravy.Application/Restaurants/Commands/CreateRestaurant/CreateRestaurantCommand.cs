using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.CreateRestaurant;

public sealed record CreateRestaurantCommand(
    string Name, 
    string Description, 
    string Email,
    string PhoneNumber,
    string Address, 
    Guid OwnerId, 
    TimeSpan[] OpeningHours) : ICommand<Guid>;
