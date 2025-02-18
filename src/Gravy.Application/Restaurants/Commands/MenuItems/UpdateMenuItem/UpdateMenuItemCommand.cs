using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Enums;

namespace Gravy.Application.Restaurants.Commands.MenuItems.UpdateMenuItem;

public sealed record UpdateMenuItemCommand(
    Guid RestaurantId,
    Guid MenuItemId,
    string Name,
    string Description,
    decimal Price,
    Category Category,
    bool IsAvailable) : ICommand;