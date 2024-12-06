using FluentValidation;

namespace Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonAvailability;

public sealed class AddDeliveryPersonAvailabilityCommandValidator : AbstractValidator<AddDeliveryPersonAvailabilityCommand>
{
    public AddDeliveryPersonAvailabilityCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.StartTimeUtc).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.EndTimeUtc).NotEmpty();
    }
}