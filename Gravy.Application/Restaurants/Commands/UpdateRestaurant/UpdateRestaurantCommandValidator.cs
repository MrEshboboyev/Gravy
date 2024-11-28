using FluentValidation;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.UpdateRestaurant;

internal class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {

        RuleFor(restaurant => restaurant.Name).NotEmpty();

        RuleFor(restaurant => restaurant.Email).NotEmpty().MaximumLength(Email.MaxLength);

        RuleFor(restaurant => restaurant.PhoneNumber).NotEmpty();

        RuleFor(restaurant => restaurant.Address).NotEmpty().MaximumLength(Address.MaxLength);
    }
}

