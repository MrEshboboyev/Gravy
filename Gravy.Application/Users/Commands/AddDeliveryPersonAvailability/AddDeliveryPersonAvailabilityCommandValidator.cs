using FluentValidation;

namespace Gravy.Application.Users.Commands.AddDeliveryPersonAvailability;

public sealed class AddDeliveryPersonAvailabilityCommandValidator : AbstractValidator<AddDeliveryPersonAvailabilityCommand>
{
    public AddDeliveryPersonAvailabilityCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.StartTimeUtc).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.EndTimeUtc).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson)
            .Must(deliveryPerson => deliveryPerson.StartTimeUtc < deliveryPerson.EndTimeUtc)
            .WithMessage("StartTimeUtc must be less than EndTimeUtc.");
    }
}