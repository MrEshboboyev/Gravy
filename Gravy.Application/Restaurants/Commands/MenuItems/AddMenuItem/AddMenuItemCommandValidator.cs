using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.MenuItems.AddMenuItem;

internal class AddMenuItemCommandValidator : AbstractValidator<AddMenuItemCommand>
{
    public AddMenuItemCommandValidator()
    {
        RuleFor(menuItem => menuItem.RestaurantId).NotEmpty();

        RuleFor(menuItem => menuItem.Name).NotEmpty();

        RuleFor(menuItem => menuItem.Price).GreaterThan(0);

        RuleFor(menuItem => menuItem.Category).NotEmpty();
    }
}
