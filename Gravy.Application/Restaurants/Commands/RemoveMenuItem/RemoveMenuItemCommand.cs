using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.RemoveMenuItem;

public sealed record RemoveMenuItemCommand(
    Guid RestaurantId,
    Guid MenuItemId) : ICommand;

