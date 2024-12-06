using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonAvailability;

public sealed record AddDeliveryPersonAvailabilityCommand(
    Guid UserId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc) : ICommand;