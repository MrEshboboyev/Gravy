using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Deliveries.CompleteDelivery;

public sealed record class CompleteDeliveryCommand(
    Guid OrderId) : ICommand;

