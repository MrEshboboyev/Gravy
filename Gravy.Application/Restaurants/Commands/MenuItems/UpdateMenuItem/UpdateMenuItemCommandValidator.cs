using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.MenuItems.UpdateMenuItem;

public sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(menuItem => menuItem.RestaurantId).NotEmpty();

        RuleFor(menuItem => menuItem.MenuItemId).NotEmpty();

        RuleFor(menuItem => menuItem.Name).NotEmpty();

        RuleFor(menuItem => menuItem.Price).GreaterThan(0);

        RuleFor(menuItem => menuItem.Category).NotEmpty();

        RuleFor(menuItem => menuItem.IsAvailable).NotEmpty();
    }
}