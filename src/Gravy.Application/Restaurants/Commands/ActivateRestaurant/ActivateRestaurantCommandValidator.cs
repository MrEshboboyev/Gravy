using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.ActivateRestaurant;

internal sealed class ActivateRestaurantCommandValidator : AbstractValidator<ActivateRestaurantCommand>
{
    public ActivateRestaurantCommandValidator()
    {
        RuleFor(restaurant => restaurant.RestaurantId).NotEmpty();
    }
}