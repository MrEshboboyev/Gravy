using FluentValidation;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;

internal sealed class AddAvailabilityCommandValidator : AbstractValidator<AddAvailabilityCommand>
{
    public AddAvailabilityCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.StartTimeUtc).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.EndTimeUtc).NotEmpty();
    }
}