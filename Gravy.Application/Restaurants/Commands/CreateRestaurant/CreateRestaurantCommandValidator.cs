using FluentValidation;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Restaurants.Commands.CreateRestaurant;

internal class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {

        RuleFor(restaurant => restaurant.Name).NotEmpty();
        
        RuleFor(restaurant => restaurant.Email).NotEmpty().MaximumLength(Email.MaxLength);

        RuleFor(restaurant => restaurant.PhoneNumber).NotEmpty();
        
        RuleFor(restaurant => restaurant.Address).NotEmpty().MaximumLength(Address.MaxLength);
        
        RuleFor(restaurant => restaurant.OwnerId).NotEmpty();

        RuleFor(restaurant => restaurant.OpeningHours).NotEmpty();
    }
}

