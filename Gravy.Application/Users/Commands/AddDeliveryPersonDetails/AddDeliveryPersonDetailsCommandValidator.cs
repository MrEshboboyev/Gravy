using FluentValidation;

namespace Gravy.Application.Users.Commands.AddDeliveryPersonDetails;

public sealed class AddDeliveryPersonDetailsCommandValidator : AbstractValidator<AddDeliveryPersonDetailsCommand>
{
    public AddDeliveryPersonDetailsCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.Type).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.LicensePlate).NotEmpty();
    }
}