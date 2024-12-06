using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;

public sealed record UpdateAvailabilityCommand(
    Guid UserId,
    Guid AvailabilityId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc) : ICommand;