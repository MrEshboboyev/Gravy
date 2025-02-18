using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;

public sealed record AddAvailabilityCommand(
    Guid UserId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc) : ICommand;