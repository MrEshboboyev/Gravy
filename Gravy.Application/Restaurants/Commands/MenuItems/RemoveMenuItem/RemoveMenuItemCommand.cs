using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;

public sealed record RemoveMenuItemCommand(
    Guid RestaurantId,
    Guid MenuItemId) : ICommand;

