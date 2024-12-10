using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid UserId,
    Guid RestaurantId,
    string Street, 
    string City, 
    string State, 
    double Latitude,
    double Longitude) : ICommand<Guid>;