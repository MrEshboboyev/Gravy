using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Deliveries.CreateDelivery;

public sealed record CreateDeliveryCommand(
    Guid OrderId) : ICommand<Guid>;