using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Enums;

namespace Gravy.Application.Restaurants.Commands.AddMenuItem;

public sealed record AddMenuItemCommand(
    Guid RestaurantId,
    string Name, 
    string Description,
    decimal Price,
    Category Category) : ICommand;

