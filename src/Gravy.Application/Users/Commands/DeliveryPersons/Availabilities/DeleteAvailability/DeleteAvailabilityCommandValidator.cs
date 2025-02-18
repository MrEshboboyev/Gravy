using FluentValidation;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;

internal sealed class DeleteAvailabilityCommandValidator : AbstractValidator<DeleteAvailabilityCommand>
{
    public DeleteAvailabilityCommandValidator()
    {
        RuleFor(deliveryPerson => deliveryPerson.UserId).NotEmpty();

        RuleFor(deliveryPerson => deliveryPerson.AvailabilityId).NotEmpty();
    }
}