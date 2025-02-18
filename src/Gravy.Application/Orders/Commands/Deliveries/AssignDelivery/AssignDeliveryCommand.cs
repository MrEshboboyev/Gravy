using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Deliveries.AssignDelivery;

public sealed record AssignDeliveryCommand(
    Guid OrderId) : ICommand;