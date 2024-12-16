using FluentValidation;

namespace Gravy.Application.Restaurants.Commands.ActivateRestaurant;

public sealed class ActivateRestaurantCommandValidator : AbstractValidator<ActivateRestaurantCommand>
{
    public ActivateRestaurantCommandValidator()
    {
        RuleFor(restaurant => restaurant.RestaurantId).NotEmpty();
    }
}