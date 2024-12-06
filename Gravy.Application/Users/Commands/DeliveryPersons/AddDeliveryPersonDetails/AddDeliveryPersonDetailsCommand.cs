using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonDetails;

public sealed record AddDeliveryPersonDetailsCommand(
    Guid UserId,
    string Type,
    string LicensePlate) : ICommand;