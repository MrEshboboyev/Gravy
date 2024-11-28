using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.AssignDelivery;

public sealed record AssignDeliveryCommand(
    Guid OrderId,
    Guid DeliveryPersonId,
    TimeSpan EstimatedDeliveryTime) : ICommand;