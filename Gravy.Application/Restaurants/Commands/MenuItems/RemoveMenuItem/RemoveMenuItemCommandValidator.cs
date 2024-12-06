using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;

internal sealed class RemoveMenuItemCommandValidator : AbstractValidator<RemoveMenuItemCommand>
{
    public RemoveMenuItemCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();

        RuleFor(x => x.MenuItemId).NotEmpty();
    }
}

