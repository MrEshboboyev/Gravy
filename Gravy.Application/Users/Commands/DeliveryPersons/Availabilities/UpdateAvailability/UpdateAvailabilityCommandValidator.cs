using FluentValidation;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;

public sealed class UpdateAvailabilityCommandValidator : AbstractValidator<UpdateAvailabilityCommand>
{
    public UpdateAvailabilityCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.AvailabilityId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.StartTimeUtc).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.EndTimeUtc).NotEmpty();
    }
}