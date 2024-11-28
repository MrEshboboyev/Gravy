using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.CompleteDelivery;

public sealed record class CompleteDeliveryCommand(
    Guid OrderId) : ICommand;

