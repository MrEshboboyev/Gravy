using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;

public sealed record DeleteAvailabilityCommand(
    Guid UserId,
    Guid AvailabilityId) : ICommand;