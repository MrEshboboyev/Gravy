using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.AddDeliveryPersonAvailability;

public sealed record AddDeliveryPersonAvailabilityCommand(
    Guid UserId,
    DateTime StartTimeUtc, 
    DateTime EndTimeUtc) : ICommand;